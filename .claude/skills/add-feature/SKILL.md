---
name: add-feature
description: Standard workflow for adding a feature to the Triggr EventBus library. Auto-invoke when the user asks to add a feature, method, or capability to EventBus / IEventBus.
---

# Add Feature to EventBus

Follow this workflow exactly. Do not commit without explicit user confirmation.

## Steps

### 1. Update `Triggr/IEventBus.cs`
Add the new method signature with XML-free, minimal declaration. Default parameters go here if applicable.

### 2. Implement in `Triggr/EventBus.cs`
Implement the interface method. No comments unless the logic is non-obvious. No extra abstractions.

### 3. Add tests in `Triggr.Tests/EventBusTests.cs`
- Pattern: **AAA** (Arrange / Act / Assert) — no comments labelling the sections
- Naming: `Method_Scenario_ExpectedResult`
  - e.g. `Subscribe_WithConditionFalse_HandlerNotCalled`
  - e.g. `Publish_WithBlockingMiddleware_HandlersNotInvoked`
- One `[Fact]` per behaviour
- Cover: happy path, edge cases, interaction with existing features (priority, middleware, condition)
- Use private records for event types inside the test class

### 4. Update `README.md` (if user-visible)
Add a usage example under the relevant section. Keep it short — one snippet, no prose.

### 5. Run tests
```
dotnet test
```
All tests must pass (0 failures, 0 warnings).

### 6. Show diff and wait
- Show `dotnet test` result
- Show diff of all changed files
- **Do not commit.** Ask the user for confirmation before committing.
