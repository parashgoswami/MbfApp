using System.Diagnostics;
using Microsoft.Playwright;

namespace MbfApp.Tests.Functional.Fixtures;

public static class BlazorPageExtensions
{
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static Task<IResponse?> GotoBlazorServerPageAsync(this IPage page, string url)
        => page.GotoAsync(
            url,
            new PageGotoOptions()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
}
