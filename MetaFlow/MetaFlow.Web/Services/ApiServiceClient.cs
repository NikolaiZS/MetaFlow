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

    public ApiServiceClient(HttpClient httpClient, ILogger<ApiServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string uri)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(uri);
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
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error posting to {Uri}: {StatusCode} {Error}", uri, response.StatusCode, error);
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
            var response = await _httpClient.PostAsJsonAsync(uri, request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error posting to {Uri}", uri);
             return false;
        }
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string uri, TRequest request)
    {
       try
        {
            var response = await _httpClient.PatchAsJsonAsync(uri, request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
             var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error patching to {Uri}: {StatusCode} {Error}", uri, response.StatusCode, error);
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
            var response = await _httpClient.DeleteAsync(uri);
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
        GetAsync<List<BoardListResponse>>($"api/boards?includeArchived={includeArchived}");
        
    public Task<BoardResponse?> CreateBoardAsync(CreateBoardRequest request) =>
        PostAsync<CreateBoardRequest, BoardResponse>("api/boards", request);

    public Task<BoardResponse?> GetBoardAsync(Guid id) =>
        GetAsync<BoardResponse>($"api/boards/{id}");
        
    // Columns
    public Task<List<ColumnListResponse>?> GetColumnsAsync(Guid boardId) =>
        GetAsync<List<ColumnListResponse>>($"api/boards/{boardId}/columns");
        
    public Task<ColumnResponse?> CreateColumnAsync(Guid boardId, CreateColumnRequest request) =>
        PostAsync<CreateColumnRequest, ColumnResponse>($"api/boards/{boardId}/columns", request);
        
    // Cards
     public Task<List<CardListResponse>?> GetCardsAsync(Guid boardId) =>
        GetAsync<List<CardListResponse>>($"api/boards/{boardId}/cards");
     
     public Task<CardResponse?> CreateCardAsync(Guid boardId, CreateCardRequest request) =>
        PostAsync<CreateCardRequest, CardResponse>($"api/boards/{boardId}/cards", request);

    public Task<CardResponse?> GetCardAsync(Guid cardId) =>
        GetAsync<CardResponse>($"api/boards/{Guid.Empty}/cards/{cardId}");
    
    // Comments
    public Task<List<CommentResponse>?> GetCommentsAsync(Guid cardId) =>
        GetAsync<List<CommentResponse>>($"api/cards/{cardId}/comments");

    public Task<CommentResponse?> CreateCommentAsync(Guid cardId, CreateCommentRequest request) =>
        PostAsync<CreateCommentRequest, CommentResponse>($"api/cards/{cardId}/comments", request);

    // Attachments
    public Task<List<AttachmentResponse>?> GetAttachmentsAsync(Guid cardId) =>
        GetAsync<List<AttachmentResponse>>($"api/cards/{cardId}/attachments");

    public Task<AttachmentResponse?> UploadAttachmentAsync(Guid cardId, UploadAttachmentRequest request) =>
        PostAsync<UploadAttachmentRequest, AttachmentResponse>($"api/cards/{cardId}/attachments", request);

    // Users
    public Task<UserResponse?> GetCurrentUserAsync() =>
        GetAsync<UserResponse>("api/users/me");
}
