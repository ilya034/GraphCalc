namespace GraphCalc.Domain.ValueObjects;

public record GraphItem(
    string Expression, 
    bool IsVisible = true
);