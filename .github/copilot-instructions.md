# Copilot Instructions

## Shared Instructions

Shared Copilot instructions, skills and prompts are maintained centrally in the
[account-level `.github` repository](https://github.com/f2calv/.github). They are deliberately not
copied here.

To load them, clone that repository and either add it to the VS Code workspace or link its
instruction, skill and prompt folders into `~/.copilot/`. If those files are unavailable, stop
rather than guessing the conventions.

Everything below is specific to this repository.

## Repository Purpose

This repository is a .NET gRPC stock-ticker playground. `Service` hosts the Greeter and stock
services, `ClientA` through `ClientD` exercise them, and `ClientLib` contains shared client code.

- Treat files under `Protos/` as the service contract and update producers and consumers together.
- Keep startup ordering explicit: start `Service` before any client.
- Keep generated protobuf output out of source control.
