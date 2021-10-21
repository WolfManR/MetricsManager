namespace MetricsManagerAPI.Models;

public record OperationResult(bool IsSuccess = true);
public record OperationResult<TResult>(TResult Result, bool IsSuccess = true) : OperationResult(IsSuccess);