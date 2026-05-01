# Architecture

## Namespace (Layer) diagram

```mermaid
flowchart TB

  subgraph P["Presentation"]
    direction LR
    P_P["Properties"]
    P_CT["Controls"]
    P_C["Converters"]
    P_DD["DesignData"]
    P_S["Services"]
    P_V["Views"]
  end

  subgraph A["Applications"]
    direction LR
    A_P["Properties"]
    A_CA["CodeAnalysis"]
    A_C["Controllers"]
    A_DM["DataModels"]
    A_H["Host"]
    A_S["Services"]
    A_VM["ViewModels"]
    A_V["Views"]
  end

  D["Domain"]

  P --> A
  A --> D

  P_C --> P_CT
  P_V --> P_CT
  P_V --> P_C

  A_C --> A_CA
  A_C --> A_DM
  A_C --> A_H
  A_C --> A_S
  A_C --> A_VM
  A_S --> A_V
  A_VM --> A_DM
  A_VM --> A_S
  A_VM --> A_V
```

## Dependency Rules

[config.nsdepcop](./config.nsdepcop)
