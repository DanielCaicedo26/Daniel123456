using Audit.EntityFramework;
using Entity.Context;
using Entity.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Utilities.Services
{
    public interface IAuditService
    {
        Task LogOperationAsync(string tableName, int recordId, string operationType, object? oldValues = null, object? newValues = null, string? additionalInfo = null);
        Task LogCreateAsync<T>(string tableName, int recordId, T newEntity, string? additionalInfo = null);
        Task LogUpdateAsync<T>(string tableName, int recordId, T oldEntity, T newEntity, string? additionalInfo = null);
        Task LogDeleteAsync(string tableName, int recordId, object? deletedEntity = null, string? additionalInfo = null);
    }

    public class AuditService : IAuditService
    {
        private readonly Entity.Context.AuditDbContext _auditContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(Entity.Context.AuditDbContext auditContext, IHttpContextAccessor httpContextAccessor)
        {
            _auditContext = auditContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogOperationAsync(string tableName, int recordId, string operationType,
            object? oldValues = null, object? newValues = null, string? additionalInfo = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var log = new ConsoleLog
            {
                TableName = tableName,
                RecordId = recordId,
                OperationType = operationType.ToUpper(),
                OldValues = oldValues != null ? JsonConvert.SerializeObject(oldValues, Formatting.Indented) : null,
                NewValues = newValues != null ? JsonConvert.SerializeObject(newValues, Formatting.Indented) : null,
                UserId = GetCurrentUserId(httpContext),
                UserName = GetCurrentUserName(httpContext),
                IpAddress = GetClientIpAddress(httpContext),
                UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString(),
                Timestamp = DateTime.UtcNow,
                AdditionalInfo = additionalInfo,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            _auditContext.ConsoleLogs.Add(log);
            await _auditContext.SaveChangesAsync();
        }

        public async Task LogCreateAsync<T>(string tableName, int recordId, T newEntity, string? additionalInfo = null)
        {
            await LogOperationAsync(tableName, recordId, "INSERT", null, newEntity, additionalInfo);
        }

        public async Task LogUpdateAsync<T>(string tableName, int recordId, T oldEntity, T newEntity, string? additionalInfo = null)
        {
            await LogOperationAsync(tableName, recordId, "UPDATE", oldEntity, newEntity, additionalInfo);
        }

        public async Task LogDeleteAsync(string tableName, int recordId, object? deletedEntity = null, string? additionalInfo = null)
        {
            await LogOperationAsync(tableName, recordId, "DELETE", deletedEntity, null, additionalInfo);
        }

        private int? GetCurrentUserId(HttpContext? httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst("id") ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        private string? GetCurrentUserName(HttpContext? httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirst(ClaimTypes.Name)?.Value ??
                       httpContext.User.FindFirst("email")?.Value ??
                       "Usuario Autenticado";
            }
            return "Sistema";
        }

        private string? GetClientIpAddress(HttpContext? httpContext)
        {
            if (httpContext == null) return null;

            var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                ip = ip.Split(',')[0].Trim();
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            return ip;
        }
    }
}