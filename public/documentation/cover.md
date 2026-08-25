# Learn F# by building a bookshop

This tour is for programmers who are new to F# and functional programming. It
starts with expressions and immutable values, then develops functions, domain
types, collections, explicit errors, modules, object-oriented interoperability,
and controlled mutation.

You do not need prior knowledge of .NET. When a lesson uses a property, method,
namespace, or other .NET-style API, it explains the relevant idea in context.

## How the tour works

Every lesson has two parts. The reading pane develops one idea through small
examples. The editor contains one complete program that you can change and run.
Do not treat the program as a finished answer: predict what it will do, alter one
thing, and let the compiler show you which assumptions were wrong.

The examples share a bookshop domain. It begins as a few primitive values and
gradually becomes a catalog, inventory, shopping carts, customers, and orders.
Early representations are intentionally simple. Later lessons replace them when
records, unions, options, results, and domain-specific types can express the same
ideas more accurately.

## What “functional-first” means here

F# supports functional, object-oriented, and imperative programming. This tour
starts with immutable data and functions because they make dependencies and
state changes easy to see. Later, it introduces classes, interfaces, exceptions,
mutable values, and loops without presenting those tools as mistakes.

The aim is practical: learn to choose data that describes the problem, write
small functions with clear types, and combine them into behavior that remains
readable as the program grows.

Begin with the first lesson. Each later lesson assumes the ideas before it.
