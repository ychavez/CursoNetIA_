using System.Globalization;
using System.Xml;
using System.Xml.Linq;

// .NET 10 file-based app; kept outside the application solution.
try
{
    if (args is ["--self-test"])
    {
        RunSelfTests();
        return 0;
    }

    if (args.Length != 2 || !decimal.TryParse(args[1], NumberStyles.Number,
        CultureInfo.InvariantCulture, out var threshold) || threshold is <= 0 or > 100)
    {
        Console.Error.WriteLine("Usage: dotnet run --file tools/CoverageGate.cs -- <merged-cobertura.xml> <minimum-percent>");
        return 2;
    }

    var coverage = ReadCoverage(XDocument.Load(args[0]));
    var passed = coverage >= threshold;
    var message = FormattableString.Invariant(
        $"Line coverage: {coverage:0.00}% (minimum {threshold:0.00}%) — {(passed ? "PASS" : "FAIL")}");
    Console.WriteLine(message);
    var summaryPath = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY");
    if (!string.IsNullOrWhiteSpace(summaryPath))
    {
        File.AppendAllText(summaryPath, $"\n**{message}**\n");
    }
    return passed ? 0 : 1;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
    or XmlException or InvalidDataException or FormatException or OverflowException)
{
    Console.Error.WriteLine($"Coverage gate failed: {exception.Message}");
    return 2;
}

static decimal ReadCoverage(XDocument document)
{
    // ReportGenerator merges overlapping reports before this gate. Its raw line
    // counts avoid rounded percentages accepting coverage just below the limit.
    var root = document.Root;
    if (root?.Name != "coverage" ||
        !long.TryParse((string?)root.Attribute("lines-covered"), NumberStyles.None,
            CultureInfo.InvariantCulture, out var covered) ||
        !long.TryParse((string?)root.Attribute("lines-valid"), NumberStyles.None,
            CultureInfo.InvariantCulture, out var total) || total <= 0 || covered > total)
    {
        throw new InvalidDataException("Expected a nonempty Cobertura report with valid line counts.");
    }
    return covered * 100m / total;
}

static void RunSelfTests()
{
    foreach (var (covered, total, expected) in new (long, long, decimal)[]
    {
        (80, 100, 80m), (7999, 10000, 79.99m), (0, 100, 0m), (100, 100, 100m)
    })
    {
        var document = XDocument.Parse($"<coverage lines-covered='{covered}' lines-valid='{total}'/>");
        if (ReadCoverage(document) != expected)
        {
            throw new InvalidDataException("Incorrect coverage calculation.");
        }
    }

    foreach (var xml in new[]
    {
        "<coverage/>", "<other lines-covered='1' lines-valid='1'/>",
        "<coverage lines-covered='0' lines-valid='0'/>",
        "<coverage lines-covered='2' lines-valid='1'/>",
        "<coverage lines-covered='-1' lines-valid='1'/>",
        "<coverage lines-covered='NaN' lines-valid='1'/>"
    })
    {
        try
        {
            ReadCoverage(XDocument.Parse(xml));
        }
        catch (InvalidDataException)
        {
            continue;
        }
        throw new InvalidDataException("An invalid coverage report was accepted.");
    }
    Console.WriteLine("Coverage gate: 10 checks passed (threshold precision and invalid/empty counts).");
}
