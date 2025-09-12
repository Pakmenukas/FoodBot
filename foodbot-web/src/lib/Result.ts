export default class Result<T, E = string> {
    private constructor(
        private readonly _data: T | null,
        private readonly _error: E | null,
        private readonly _isSuccess: boolean
    ) {}

    static success<T, E = string>(data: T): Result<T, E> {
        return new Result<T, E>(data, null, true);
    }

    static failure<T, E = string>(error: E): Result<T, E> {
        return new Result<T, E>(null, error, false);
    }

    get data(): T | null {
        return this._data;
    }

    get error(): E | null {
        return this._error;
    }

    get isSuccess(): boolean {
        return this._isSuccess;
    }

    get isFailure(): boolean {
        return !this._isSuccess;
    }

    hasData(): this is Result<T, E> & { data: T } {
        return this._isSuccess && this._data !== null;
    }

    hasError(): this is Result<T, E> & { error: E } {
        return !this._isSuccess && this._error !== null;
    }

    map<U>(fn: (data: T) => U): Result<U, E> {
        if (this.isSuccess && this._data !== null) {
            try {
                return Result.success(fn(this._data));
            } catch (error) {
                return Result.failure(error as E);
            }
        }
        return Result.failure(this._error as E);
    }

    flatMap<U>(fn: (data: T) => Result<U, E>): Result<U, E> {
        if (this.isSuccess && this._data !== null) {
            try {
                return fn(this._data);
            } catch (error) {
                return Result.failure(error as E);
            }
        }
        return Result.failure(this._error as E);
    }

    mapError<F>(fn: (error: E) => F): Result<T, F> {
        if (this.isFailure && this._error !== null) {
            return Result.failure(fn(this._error));
        }
        return Result.success(this._data as T);
    }

    getOrElse(defaultValue: T): T {
        return this.isSuccess && this._data !== null ? this._data : defaultValue;
    }

    getOrThrow(errorMessage?: string): T {
        if (this.isSuccess && this._data !== null) {
            return this._data;
        }
        throw new Error(errorMessage || String(this._error));
    }

    match<U>(onSuccess: (data: T) => U, onFailure: (error: E) => U): U {
        if (this.isSuccess && this._data !== null) {
            return onSuccess(this._data);
        }
        return onFailure(this._error as E);
    }

    onSuccess(fn: (data: T) => void): this {
        if (this.isSuccess && this._data !== null) {
            fn(this._data);
        }
        return this;
    }

    onFailure(fn: (error: E) => void): this {
        if (this.isFailure && this._error !== null) {
            fn(this._error);
        }
        return this;
    }

    toJSON() {
        return {
            data: this._data,
            error: this._error,
            isSuccess: this._isSuccess
        };
    }
}

export type StringResult<T> = Result<T, string>;
export type ErrorResult<T> = Result<T, Error>;

export const Ok = Result.success;
export const Err = Result.failure;