using System.Net.Http.Json;
using System.Text.Json;
using MetaFlow.Contracts.Boards;
using MetaFlow.Contracts.Cards;
using MetaFlow.Contracts.Columns;
using MetaFlow.Contracts.Methodologies;
using MetaFlow.Contracts.Users;
using MetaFlow.Contracts.Comments;
using MetaFlow.Contracts.Attachments;

namespace MetaFlow.Web.Services;

public class ApiServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiServiceClient> _logger;
    private readonly AppState _appState;

    public ApiServiceClient(HttpClient httpClient, ILogger<ApiServiceClient> logger, AppState appState)
    {
        _httpClient = httpClient;
        _logger = logger;
        _appState = appState;
    }

    public async Task<T?> GetAsync<T>(string uri)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("GET", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {Uri}", uri);
            return default;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string uri, TRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("POST", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            _logger.LogError("Error posting to {Uri}: {StatusCode} {Error}", uri, response.StatusCode, content);
            return default;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error posting to {Uri}", uri);
             return default;
        }
    }
    
    public async Task<bool> PostVoidAsync<TRequest>(string uri, TRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("POST", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error posting to {Uri}", uri);
             return false;
        }
    }

    public async Task<bool> PutVoidAsync<TRequest>(string uri, TRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync(uri, request);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("PUT", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error putting to {Uri}", uri);
             return false;
        }
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string uri, TRequest request)
    {
       try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PatchAsJsonAsync(uri, request);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("PATCH", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            _logger.LogError("Error patching to {Uri}: {StatusCode} {Error}", uri, response.StatusCode, content);
            return default;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error patching to {Uri}", uri);
             return default;
        }
    }

    public async Task<bool> DeleteAsync(string uri)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            _appState.UpdateLastResponse("DELETE", uri, (int)response.StatusCode, content, GetHeaders(), GetResponseHeaders(response));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Uri}", uri);
            return false;
        }
    }
    
    // Auth
    public Task<AuthResponse?> LoginAsync(LoginRequest request) => 
        PostAsync<LoginRequest, AuthResponse>("api/auth/login", request);

    public Task<AuthResponse?> RegisterAsync(RegisterRequest request) => 
        PostAsync<RegisterRequest, AuthResponse>("api/auth/register", request);
        
    // Methodologies
    public Task<List<MethodologyResponse>?> GetMethodologiesAsync() =>
        GetAsync<List<MethodologyResponse>>("api/methodologies");
        
    // Boards
    public Task<List<BoardListResponse>?> GetBoardsAsync(bool includeArchived = false) =>
        GetAsync<List<BoardListResponse>>($"api/boards?includeArchived={includeArchived.ToString().ToLower()}");
        
    public Task<BoardResponse?> CreateBoardAsync(CreateBoardRequest request) =>
        PostAsync<CreateBoardRequest, BoardResponse>("api/boards", request);

    public Task<BoardResponse?> GetBoardAsync(Guid id) =>
        GetAsync<BoardResponse>($"api/boards/{id}");
        
    // Columns
    public Task<List<ColumnListResponse>?> GetColumnsAsync(Guid boardId) =>
        GetAsync<List<ColumnListResponse>>($"api/boards/{boardId}/columns");
        
    public Task<ColumnResponse?> CreateColumnAsync(Guid boardId, CreateColumnRequest request) =>
        PostAsync<CreateColumnRequest, ColumnResponse>($"api/boards/{boardId}/columns", request);
        
    public Task<ColumnResponse?> UpdateColumnAsync(Guid boardId, Guid columnId, UpdateColumnRequest request) =>
        PatchAsync<UpdateColumnRequest, ColumnResponse>($"api/boards/{boardId}/columns/{columnId}", request);
        
    public Task<bool> DeleteColumnAsync(Guid boardId, Guid columnId) =>
        DeleteAsync($"api/boards/{boardId}/columns/{columnId}");

    public Task<bool> ReorderColumnsAsync(Guid boardId, ReorderColumnsRequest request) =>
        PutVoidAsync<ReorderColumnsRequest>($"api/boards/{boardId}/columns/reorder", request); 
        
    // Cards
    public Task<List<CardListResponse>?> GetCardsAsync(Guid boardId, bool includeArchived = false) =>
        GetAsync<List<CardListResponse>>($"api/boards/{boardId}/cards?includeArchived={includeArchived.ToString().ToLower()}");
     
    public Task<CardResponse?> CreateCardAsync(Guid boardId, CreateCardRequest request) =>
        PostAsync<CreateCardRequest, CardResponse>($"api/boards/{boardId}/cards", request);

    public Task<CardResponse?> GetCardAsync(Guid cardId) =>
        GetAsync<CardResponse>($"api/boards/{Guid.Empty}/cards/{cardId}");

    //
    public Task<bool> ArchiveCardAsync(Guid boardId, Guid cardId) =>
        PutVoidAsync<object?>($"api/boards/{boardId}/cards/{cardId}/archive", null);

    public Task<bool> DeleteCardAsync(Guid boardId, Guid cardId) =>
        DeleteAsync($"api/boards/{boardId}/cards/{cardId}");
        
    public Task<CardResponse?> UpdateCardAsync(Guid boardId, Guid cardId, UpdateCardRequest request) =>
        PatchAsync<UpdateCardRequest, CardResponse>($"api/boards/{boardId}/cards/{cardId}", request);

    public Task<bool> MoveCardAsync(Guid boardId, Guid cardId, MoveCardRequest request) =>
        PutVoidAsync<MoveCardRequest>($"api/boards/{boardId}/cards/{cardId}/move", request);

    public Task<bool> UnarchiveCardAsync(Guid boardId, Guid cardId) =>
        PutVoidAsync<object?>($"api/boards/{boardId}/cards/{cardId}/unarchive", null); 
    // Comments
    public Task<List<CommentResponse>?> GetCommentsAsync(Guid cardId) =>
        GetAsync<List<CommentResponse>>($"api/cards/{cardId}/comments");

    public Task<CommentResponse?> CreateCommentAsync(Guid cardId, CreateCommentRequest request) =>
        PostAsync<CreateCommentRequest, CommentResponse>($"api/cards/{cardId}/comments", request);
        
    public Task<CommentResponse?> UpdateCommentAsync(Guid cardId, Guid commentId, UpdateCommentRequest request) =>
        PatchAsync<UpdateCommentRequest, CommentResponse>($"api/cards/{cardId}/comments/{commentId}", request);
        
    public Task<bool> DeleteCommentAsync(Guid cardId, Guid commentId) =>
        DeleteAsync($"api/cards/{cardId}/comments/{commentId}");

    // Attachments
    public Task<List<AttachmentResponse>?> GetAttachmentsAsync(Guid cardId) =>
        GetAsync<List<AttachmentResponse>>($"api/cards/{cardId}/attachments");

    public Task<AttachmentResponse?> UploadAttachmentAsync(Guid cardId, UploadAttachmentRequest request) =>
        PostAsync<UploadAttachmentRequest, AttachmentResponse>($"api/cards/{cardId}/attachments", request);
        
    public Task<bool> DeleteAttachmentAsync(Guid cardId, Guid attachmentId) =>
        DeleteAsync($"api/cards/{cardId}/attachments/{attachmentId}");

    // Users
    public Task<UserResponse?> GetCurrentUserAsync() =>
        GetAsync<UserResponse>("api/users/me");

    private void SetAuthorizationHeader()
    {
        if (_appState.IsLoggedIn && !string.IsNullOrEmpty(_appState.CurrentUser?.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.CurrentUser.Token);
        }
    }

    private Dictionary<string, IEnumerable<string>> GetHeaders()
    {
        return _httpClient.DefaultRequestHeaders.ToDictionary(h => h.Key, h => h.Value);
    }
    
    private Dictionary<string, IEnumerable<string>> GetResponseHeaders(HttpResponseMessage response)
    {
        var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);
        if (response.Content != null)
        {
            foreach (var header in response.Content.Headers)
            {
                headers[header.Key] = header.Value;
            }
        }
        return headers;
    }
}
