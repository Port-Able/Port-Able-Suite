# From [*Port-Able Suite*](.) to [*Apportia*](https://github.com/Apportia/Apportia)

[*Port-Able Suite*](.) has been discontinued in favor of [*Apportia*](https://github.com/Apportia/Apportia). The last state of the codebase is preserved in the [21.4.30 branch](https://github.com/Port-Able/Port-Able-Suite/tree/21.4.30).

**TL;DR** — *Port-Able Suite* grew without a plan, accumulated technical debt over the years, and was deeply tied to .NET Framework — a platform Microsoft eventually discontinued. Continuing it was no longer viable. *Apportia* is the rebuild: a single binary, modern .NET, support for both Windows and Linux, built for performance, with a feature-rich interface designed to keep everything fast and accessible.

<p align="center"><img src="https://raw.githubusercontent.com/Apportia/Apportia/refs/heads/main/preview.png"></p>

## Why

[*Port-Able Suite*](.) never started with a clear vision. The first version was little more than a small window with a dropdown box listing applications — essentially an "Open With" dialog, nothing more. Apps had to be added manually. Features were added gradually over time, each one building on the last without an overarching plan. The project was never designed; it accumulated.

That showed. The codebase reflected years of shifting ideas rather than deliberate architecture, and some of those early decisions created permanent maintenance burdens.

One example: due to the unreliability of PortableApps.com and SourceForge at the time, a significant portion of the infrastructure ran through self-managed servers. The idea was sound in principle, but it created a constant operational overhead that never really went away.

Another example: app icons were stored directly inside a precompiled data block and read from there at runtime. It made sense for performance at the time, but maintaining it was anything but comfortable. Every icon had to be added or removed manually, and while internal tooling made it somewhat manageable, it was a self-inflicted construction site that never went away. There were other decisions of the same kind — each one reasonable in isolation, but together they added up to a foundation that was increasingly difficult to work with.

And then there was .NET Framework.

When it became clear that .NET Framework had no future, motivation hit rock bottom. It felt like standing in a house you built yourself after a natural disaster — wanting to save it, but realizing the only real option is to tear it down and start over.

So that is exactly what happened: a complete restart, built on the right foundation from day one.

## What is [*Apportia*](https://github.com/Apportia/Apportia)

[*Apportia*](https://github.com/Apportia/Apportia) follows a similar concept to [*Port-Able Suite*](.), but the implementation and architecture are fundamentally different. It is built on modern .NET and supports both Windows and Linux.

[*Port-Able Suite*](.) consisted of several separate programs: a Launcher that displayed and managed installed applications, a Downloader that handled installation and updates, and an Updater responsible solely for keeping the suite itself up to date. On top of that, it carried additional dependencies such as a bundled 7-Zip and other external components.

[*Apportia*](https://github.com/Apportia/Apportia) replaces all of that with a single binary — `Apportia` on Linux, `Apportia.exe` on Windows. No separate tools, no bundled dependencies. It is faster, more secure, and designed to be more intuitive to use.

## Migrating from [*Port-Able Suite*](.)

> **Note:** The *Port-Able Suite* servers will be shut down soon. If you are still using *Port-Able Suite*, now is a good time to switch.

Most applications can be copied directly from your [*Port-Able Suite*](.) Apps folder and will work without any additional steps. The only exceptions are applications located in the `.free`, `.repack`, and `.share` folders — those need to be added through the **Import App** feature instead.

## Suggestions and Feedback

If you have suggestions or feature requests for [*Apportia*](https://github.com/Apportia/Apportia), feel free to open an issue on the [*Apportia* repository](https://github.com/Apportia/Apportia).

Please note that requests related to creating or packaging portable applications are out of scope. [*Apportia*](https://github.com/Apportia/Apportia) manages and deploys apps from an existing source — it is not responsible for building or providing the apps themselves.

That said, dedicated tooling for creating portable applications is planned under the  [*Apportia* organization](https://github.com/Apportia), including Linux support. It will happen — it is just a matter of finding the time.
