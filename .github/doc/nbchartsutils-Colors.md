# Colors `class`

## Diagram
```mermaid
  flowchart LR
  classDef interfaceStyle stroke-dasharray: 5 5;
  classDef abstractStyle stroke-width:4px
  subgraph NB.Charts.Utils
  NB.Charts.Utils.Colors[[Colors]]
  end
```

## Members
### Methods
#### Public Static methods
| Returns | Name |
| --- | --- |
| `Color` | [`FromPalette`](#frompalette)([`Palettes`](./nbcharts-Palettes.md) palette, `int` idx) |
| `Color` | [`RandNice`](#randnice)()<br>Generates a random but nice-looking color |

## Details
### Methods
#### RandNice
```csharp
public static Color RandNice()
```
##### Summary
Generates a random but nice-looking color

#### FromPalette
```csharp
public static Color FromPalette(Palettes palette, int idx)
```
##### Arguments
| Type | Name | Description |
| --- | --- | --- |
| [`Palettes`](./nbcharts-Palettes.md) | palette |   |
| `int` | idx |   |
