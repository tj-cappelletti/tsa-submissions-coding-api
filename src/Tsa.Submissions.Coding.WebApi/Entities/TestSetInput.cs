﻿namespace Tsa.Submissions.Coding.WebApi.Entities;

public class TestSetInput
{
    public string? DataType { get; set; }

    public int? Index { get; set; }

    public bool IsArray { get; set; }

    public string? ValueAsJson { get; set; }
}
