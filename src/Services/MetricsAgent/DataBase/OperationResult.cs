namespace MetricsAgent.DataBase;

public record OperationResult(bool IsSuccess = true);
public record OperationResult<TResult>(bool IsSuccess = true, TResult? Result = default) : OperationResult(IsSuccess);