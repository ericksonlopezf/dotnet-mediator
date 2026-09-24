// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.Mediator;

/// <summary>
/// Represents a type with a single value, typically used to indicate the absence of a return value in generic signatures.
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    private static readonly Unit _value = default;

    /// <summary>
    /// Gets the single <see cref="Unit"/> value.
    /// </summary>
    public static ref readonly Unit Value => ref _value;

    /// <inheritdoc />
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    int IComparable.CompareTo(object? obj) => obj is Unit ? 0 : 1;

    /// <inheritdoc />
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <summary>
    /// Determines whether two <see cref="Unit"/> values are equal.
    /// </summary>
    /// <param name="first">The first instance to compare.</param>
    /// <param name="second">The second instance to compare.</param>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool operator ==(Unit first, Unit second) => true;

    /// <summary>
    /// Determines whether two <see cref="Unit"/> values are not equal.
    /// </summary>
    /// <param name="first">The first instance to compare.</param>
    /// <param name="second">The second instance to compare.</param>
    /// <returns>Always <see langword="false"/>.</returns>
    public static bool operator !=(Unit first, Unit second) => false;

    /// <summary>
    /// Determines whether the first <see cref="Unit"/> is less than the second.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always <see langword="false"/>.</returns>
    public static bool operator <(Unit left, Unit right) => false;

    /// <summary>
    /// Determines whether the first <see cref="Unit"/> is less than or equal to the second.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool operator <=(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether the first <see cref="Unit"/> is greater than the second.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always <see langword="false"/>.</returns>
    public static bool operator >(Unit left, Unit right) => false;

    /// <summary>
    /// Determines whether the first <see cref="Unit"/> is greater than or equal to the second.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool operator >=(Unit left, Unit right) => true;
}
