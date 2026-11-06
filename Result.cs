using System;
using System.Collections.Generic;
using System.Linq;
using ReCo.Result.Interfaces;

namespace ReCo.Result;

/// <summary>
/// Represents the outcome of an operation that produces a value of type <typeparamref name="T"/>,
/// containing either the produced value or a collection of errors.
/// </summary>
/// <typeparam name="T">The type of the encapsulated value.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the payload value when the operation succeeds, or <see langword="null"/> if failed or not set.
    /// </summary>
    public T? Value { get; private set; }

    private Result(T value) => Value = value;

    private Result()
    {
    }

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value produced by the operation.</param>
    /// <returns>A successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    /// <param name="message">The description of the error.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static new Result<T> Fail(string message) => Fail(new Error(message));

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error details.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static new Result<T> Fail(IError error)
    {
        var result = new Result<T>();
        result._errors.Add(error);
        return result;
    }

    /// <summary>
    /// Creates a failed result containing multiple error messages.
    /// </summary>
    /// <param name="messages">A collection of error descriptions.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static new Result<T> Fail(IEnumerable<string> messages)
    {
        var result = new Result<T>();
        result._errors.AddRange(messages.Select(msg => new Error(msg)));
        return result;
    }

    /// <summary>
    /// Creates a failed result containing multiple errors.
    /// </summary>
    /// <param name="errors">A collection of errors.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static new Result<T> Fail(IEnumerable<IError> errors)
    {
        var result = new Result<T>();
        result._errors.AddRange(errors);
        return result;
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> into a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<T>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return Fail(error);
    }

    /// <summary>
    /// Appends an error to the current result.
    /// </summary>
    /// <param name="error">The error to append.</param>
    /// <returns>The current instance with the added error.</returns>
    public new Result<T> WithError(IError error)
    {
        base.WithError(error);
        return this;
    }

    /// <summary>
    /// Appends an error message to the current result.
    /// </summary>
    /// <param name="message">The error message to append.</param>
    /// <returns>The current instance with the added error.</returns>
    public new Result<T> WithError(string message) => WithError(new Error(message));

    /// <summary>
    /// Appends a collection of errors to the current result.
    /// </summary>
    /// <param name="errors">The errors to append.</param>
    /// <returns>The current instance with the added errors.</returns>
    public new Result<T> WithErrors(IEnumerable<IError> errors)
    {
        base.WithErrors(errors);
        return this;
    }

    /// <summary>
    /// Appends a collection of error messages to the current result.
    /// </summary>
    /// <param name="messages">The error messages to append.</param>
    /// <returns>The current instance with the added errors.</returns>
    public new Result<T> WithErrors(IEnumerable<string> messages)
    {
        base.WithErrors(messages);
        return this;
    }

    /// <summary>
    /// Appends an informational success message to the current result.
    /// </summary>
    /// <param name="message">The success message to append.</param>
    /// <returns>The current instance with the added success message.</returns>
    public new Result<T> WithSuccess(string message)
    {
        base.WithSuccess(message);
        return this;
    }

    /// <summary>
    /// Executes the specified delegate within a try-catch block, returning a success result on completion
    /// or a failure result if an unhandled exception occurs.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="errorHandler">An optional handler to map caught exceptions to domain errors.</param>
    /// <returns>A successful result containing the computed value, or a failure result with error details.</returns>
    public static Result<T> Try(Func<T> func, Func<Exception, IError>? errorHandler = null)
    {
        try
        {
            var value = func();
            return Success(value);
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex) ?? Error.Internal("Internal server error.");
            return Fail(error);
        }
    }
}

/// <summary>
/// Represents the outcome of an operation that does not return a value,
/// indicating success or containing a collection of errors.
/// </summary>
public class Result
{
    /// <summary>
    /// The backing list storing errors associated with this result.
    /// </summary>
    protected readonly List<IError> _errors = new();

    /// <summary>
    /// The backing list storing success/informational messages associated with this result.
    /// </summary>
    protected readonly List<string> _successes = new();

    /// <summary>
    /// Gets a value indicating whether the operation completed without errors.
    /// </summary>
    public bool IsSuccess => !_errors.Any();

    /// <summary>
    /// Gets a value indicating whether the operation encountered one or more errors.
    /// </summary>
    public bool IsFailure => _errors.Any();

    /// <summary>
    /// Gets a read-only view of the errors encountered during the operation.
    /// </summary>
    public IReadOnlyCollection<IError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Gets a read-only view of informational success messages recorded during the operation.
    /// </summary>
    public IReadOnlyCollection<string> Successes => _successes.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    protected Result()
    {
    }

    /// <summary>
    /// Creates a successful result without a payload.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    public static Result Success() => new();

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    /// <param name="message">The description of the error.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(string message) => Fail(new Error(message));

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error details.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(IError error)
    {
        var result = new Result();
        result._errors.Add(error);
        return result;
    }

    /// <summary>
    /// Creates a failed result containing multiple error messages.
    /// </summary>
    /// <param name="messages">A collection of error descriptions.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(IEnumerable<string> messages)
    {
        var result = new Result();
        result._errors.AddRange(messages.Select(msg => new Error(msg)));
        return result;
    }

    /// <summary>
    /// Creates a failed result containing multiple errors.
    /// </summary>
    /// <param name="errors">A collection of errors.</param>
    /// <returns>A failed <see cref="Result"/> instance.</returns>
    public static Result Fail(IEnumerable<IError> errors)
    {
        var result = new Result();
        result._errors.AddRange(errors);
        return result;
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> into a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return Fail(error);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Result"/> instance to a boolean flag representing success.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    public static implicit operator bool(Result? result) => result is not null && result.IsSuccess;

    /// <summary>
    /// Appends an error to the current result.
    /// </summary>
    /// <param name="error">The error to append.</param>
    /// <returns>The current instance with the added error.</returns>
    public Result WithError(IError error)
    {
        _errors.Add(error);
        return this;
    }

    /// <summary>
    /// Appends an error message to the current result.
    /// </summary>
    /// <param name="message">The error message to append.</param>
    /// <returns>The current instance with the added error.</returns>
    public Result WithError(string message) => WithError(new Error(message));

    /// <summary>
    /// Appends a collection of errors to the current result.
    /// </summary>
    /// <param name="errors">The errors to append.</param>
    /// <returns>The current instance with the added errors.</returns>
    public Result WithErrors(IEnumerable<IError> errors)
    {
        _errors.AddRange(errors);
        return this;
    }

    /// <summary>
    /// Appends a collection of error messages to the current result.
    /// </summary>
    /// <param name="messages">The error messages to append.</param>
    /// <returns>The current instance with the added errors.</returns>
    public Result WithErrors(IEnumerable<string> messages)
    {
        _errors.AddRange(messages.Select(msg => new Error(msg)));
        return this;
    }

    /// <summary>
    /// Appends an informational success message to the current result.
    /// </summary>
    /// <param name="message">The success message to append.</param>
    /// <returns>The current instance with the added success message.</returns>
    public Result WithSuccess(string message)
    {
        _successes.Add(message);
        return this;
    }

    /// <summary>
    /// Executes the specified action within a try-catch block, returning a success result on completion
    /// or a failure result if an unhandled exception occurs.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="errorHandler">An optional handler to map caught exceptions to domain errors.</param>
    /// <returns>A successful result, or a failure result with error details.</returns>
    public static Result Try(Action action, Func<Exception, IError>? errorHandler = null)
    {
        try
        {
            action();
            return Success();
        }
        catch (Exception ex)
        {
            var error = errorHandler?.Invoke(ex) ?? Error.Internal("Internal server error.");
            return Fail(error);
        }
    }
}