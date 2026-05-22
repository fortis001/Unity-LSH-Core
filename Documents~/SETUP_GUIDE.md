# Project Setup Guide

이 문서는 Core 패키지를 새 Unity 프로젝트에 적용할 때 필요한 기본 세팅 순서를 정리한 가이드입니다.

---

## 1. 기본 폴더 구조 생성

프로젝트의 `Assets` 폴더 아래에 다음 폴더 구조를 생성합니다.

```txt
Assets
├─ 1_Scenes
├─ 2_Scripts
│  ├─ 0_Core
│  ├─ 1_Actors
│  ├─ 2_GamePlay
│  ├─ 3_UI
│  ├─ 4_Data
│  └─ 9_Utils
├─ 3_Resources
└─ Settings
   ├─ Scenes
   ├─ Sound
   └─ System
```

---

## 2. 기본 씬 생성

`Assets/1_Scenes` 폴더에 다음 씬을 생성합니다.

```txt
00_Entry
01_Title
02_InGame
99_Loading
```

각 씬의 용도는 다음과 같습니다.

```txt
00_Entry
- 앱 초기화용 진입 씬
- Core 매니저와 Bootstrapper가 배치됩니다.

01_Title
- 타이틀 씬

02_InGame
- 실제 게임 플레이 씬

99_Loading
- 씬 전환 중 사용할 로딩 씬
```

---

## 3. Core용 프로젝트 스크립트 생성

`Assets/2_Scripts/0_Core` 폴더에 다음 스크립트를 생성합니다.

```txt
BootstrapContext
SceneNames
SoundIDs
TimeChannelName
```

해당 스크립트들은 패키지에서 제공하는 템플릿을 복사하여 프로젝트에 맞게 생성합니다.

### 예시 역할

```txt
BootstrapContext
- Core 초기화에 필요한 프로젝트별 설정을 제공하는 Context입니다.

SceneNames
- 프로젝트에서 사용할 씬 이름을 SceneReference로 관리합니다.

SoundIDs
- 프로젝트에서 사용할 BGM/SFX ID를 정의합니다.

TimeChannelName
- 프로젝트에서 사용할 TimeChannel 이름을 정의합니다.
```

---

## 4. Scene Settings 생성

`Assets/Settings/Scenes` 폴더에 `CoreSceneSettings`를 생성합니다.

생성 후 다음 씬을 연결합니다.

```txt
Entry Complete Scene : 01_Title
Loading Scene        : 99_Loading
Fallback Scene       : 01_Title
```

일반적으로 초기화가 끝난 뒤 타이틀 씬으로 이동하므로, `Entry Complete Scene`과 `Fallback Scene`은 `01_Title`로 설정합니다.

---

## 5. Sound Settings 생성

`Assets/Settings/Sound` 폴더에 `SoundData`를 생성합니다.

`SoundData`에는 프로젝트에서 사용할 BGM과 SFX 정보를 등록합니다.

```txt
BGM List
- BGM SoundId
- AudioClip
- Volume

SFX List
- SFX SoundId
- AudioClip
- Volume
```

---

## 6. Input System 설정

`Assets/Settings/System` 폴더에 `InputAction` 맵을 생성합니다.

이후 패키지에서 제공하는 InputAction 스크립트 템플릿을 복사하여 프로젝트에 맞게 생성합니다.

예시:

```txt
InputAction Asset
- Title
- InGame
- UI
```

프로젝트에서 필요한 Action Map과 Action을 정의한 뒤, 생성된 InputAction Asset을 `InputManager`에 연결합니다.

---

## 7. Entry 씬 구성

`00_Entry` 씬에 초기화용 오브젝트를 생성합니다.

권장 구조는 다음과 같습니다.

```txt
AppBootstrapper
├─ BootContext
├─ InputManager
├─ SoundManager
│  ├─ BGM Source
│  └─ SFX Source
├─ TimeManager
└─ TransitionManager
   └─ FadeCanvasGroup
      └─ Canvas
         └─ FadePanel
```

---

## 8. BootstrapContext 설정

`AppBootstrapper` 하위에 `BootContext` 오브젝트를 생성합니다.

`BootContext` 오브젝트에 프로젝트용 `BootstrapContext` 스크립트를 부착합니다.

그 후 `BootstrapContext`에 `CoreSceneSettings`를 연결합니다.

```txt
BootContext
└─ BootstrapContext
   └─ CoreSceneSettings
```

---

## 9. InputManager 설정

`InputManager` 오브젝트를 생성하고 `InputManager` 컴포넌트를 부착합니다.

이후 `InputAction Asset` 필드에 앞에서 생성한 InputAction 맵을 연결합니다.

```txt
InputManager
└─ InputAction Asset
```

---

## 10. SoundManager 설정

`SoundManager` 오브젝트를 생성하고 `SoundManager` 컴포넌트를 부착합니다.

`SoundManager` 하위에 다음 오브젝트를 생성합니다.

```txt
SoundManager
├─ BGM Source
└─ SFX Source
```

각 하위 오브젝트에 `AudioSource` 컴포넌트를 부착합니다.

`BGM Source`와 `SFX Source`의 `Play On Awake` 옵션은 해제합니다.

이후 `SoundManager`에 다음 항목을 연결합니다.

```txt
Sound Data : SoundData
BGM Source : BGM Source의 AudioSource
SFX Source : SFX Source의 AudioSource
```

---

## 11. TransitionManager 설정

`TransitionManager` 오브젝트를 생성하고 `TransitionManager` 컴포넌트를 부착합니다.

`TransitionManager` 하위에 페이드용 오브젝트를 생성합니다.

```txt
TransitionManager
└─ FadeCanvasGroup
   └─ Canvas
      └─ FadePanel
```

`FadeCanvasGroup` 오브젝트에는 `CanvasGroup` 컴포넌트를 부착합니다.

`FadeCanvasGroup` 하위에 `Image` 오브젝트를 생성하고, 전체 화면을 덮을 수 있도록 크기를 충분히 크게 설정합니다.

`Canvas`의 `Sort Order`를 충분히 높게 설정합니다.

예시:

```txt
FadePanel
- Image
- Color: Black
- RectTransform: 화면 전체를 덮도록 설정
```

이후 `TransitionManager`의 CanvasGroup 필드에 `FadeCanvasGroup`을 연결합니다.

---

## 12. AppBootstrapper 설정

`AppBootstrapper` 오브젝트에 `AppBootstrapper` 컴포넌트를 부착합니다.

다음 필드를 연결합니다.

```txt
Context
- BootContext의 BootstrapContext

Managers
- InputManager
- SoundManager
- TimeManager
- TransitionManager
```

`Managers` 필드에는 초기화가 필요한 매니저들을 등록합니다.

등록된 매니저 중 `IBootable` 또는 `IBootableWithContext`를 구현한 매니저는 앱 시작 시 `AppBootstrapper`에 의해 자동으로 초기화됩니다.

---

## 13. SceneLoader 배치

99_LoadingScene에 `SceneLoader` 오브젝트를 생성하고 `SceneLoader` 컴포넌트를 부착합니다.

`ProgressBar`와 `ProgressText`는 비워두어도 동작하지만, 동작 진행도를 나타내고 싶을 때 연결합니다.

---

## 14. Build Settings 확인

Unity의 `Build Settings`에 다음 씬을 등록합니다.

```txt
00_Entry
01_Title
02_InGame
99_Loading
```

`00_Entry` 씬이 가장 먼저 실행되도록 빌드 인덱스 0번에 배치합니다.

```txt
0 : 00_Entry
1 : 01_Title
2 : 02_InGame
3 : 99_Loading
```

---

## 15. 기본 실행 흐름

기본 실행 흐름은 다음과 같습니다.

```txt
00_Entry 실행
→ AppBootstrapper 초기화
→ Core Managers 초기화
→ CoreSceneSettings의 Entry Complete Scene으로 이동
→ 01_Title 씬 진입
```

---

## Setup Checklist

설정이 끝난 뒤 아래 항목을 확인합니다.

```txt
[ ] Assets 폴더 구조를 생성했는가?
[ ] 00_Entry, 01_Title, 02_InGame, 99_Loading 씬을 생성했는가?
[ ] Core용 프로젝트 스크립트 템플릿을 복사했는가?
[ ] CoreSceneSettings를 생성하고 씬을 연결했는가?
[ ] SoundData를 생성했는가?
[ ] InputAction Asset을 생성했는가?
[ ] Entry 씬에 AppBootstrapper를 생성했는가?
[ ] BootContext에 BootstrapContext를 부착했는가?
[ ] InputManager에 InputAction Asset을 연결했는가?
[ ] SoundManager에 SoundData와 AudioSource를 연결했는가?
[ ] AudioSource의 Play On Awake를 해제했는가?
[ ] TransitionManager에 CanvasGroup을 연결했는가?
[ ] FadePanel이 화면 전체를 덮도록 설정했는가?
[ ] AppBootstrapper에 Context와 Managers를 연결했는가?
[ ] 99_Loading 씬에 SceneLoader를 생성했는가?
[ ] Build Settings에 씬을 등록했는가?
[ ] 00_Entry 씬이 빌드 인덱스 0번인가?
```
