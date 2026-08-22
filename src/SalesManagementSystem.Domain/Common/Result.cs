using System;
using System.Diagnostics;

namespace SalesManagementSystem.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }

    public string? ErrorMessage { get; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? errorMessage)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(errorMessage))
            throw new InvalidOperationException("A successful result cannot have an error message.");
        if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
            throw new InvalidOperationException("A failed result must have an error message.");

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string errorMessage) => new(false, errorMessage);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string errorMessage) => Result<T>.Failure(errorMessage);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    protected Result(bool isSuccess, T? value, string? errorMessage)
        : base(isSuccess, errorMessage)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public new static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
}
