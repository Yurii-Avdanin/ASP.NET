#nullable enable

namespace Pcf.ReceivingFromPartner.Core.Common;

public record Response<T>
{
    private Response() { }

    public static Response<T> Success(T data) => new Response<T> { IsSuccess = true, Data = data };

    public static Response<T> Failure(string message) => new Response<T> { IsSuccess = false, ErrMsg = message };

    public T? Data { get; init; } = default;   

    public bool IsSuccess { get; init; } = false;

    public string ErrMsg { get; init; } = string.Empty;
}
