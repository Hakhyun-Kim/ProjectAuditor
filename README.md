# Project Auditor
Project Auditor 는 유니티 프로젝트를 정적으로 분석해주는 툴입니다. Project Auditor 는 스크립트와 세팅을 분석하여 성능에 문제가 될만한 부분을 찾아줍니다. 

### Current Status
이 프로젝트는 Experimental 상태이며 추후에 많이 변경될 가능성이 높습니다. 특정 버전의 유니티와 제한된 컨텐츠에서 사용가능합니다.

### Compatibility
모든 유니티 버전에 사용가능합니다. 좀 더 자세한 사항은 뒤의 Installation 을 참조하세요.

### Disclaimer
이 프로젝트는 유니티 개발자가 개발했습니다만, 유니티에서 공식적으로 서포트하지 않습니다. 

### License
Project Auditor is licensed under the [Unity Package Distribution License](LICENSE.md) as of November 18th 2020. Before then, the MIT license was in play.

## Installation
Project Auditor는 2018 이후 버전부터 패키지로 설치 가능합니다.

Add `com.unity.project-auditor` as a dependency to the project `Packages/manifest.json` file:

```
{
  "dependencies": {
    "com.unity.project-auditor": "https://github.com/Unity-Technologies/ProjectAuditor.git",
  }
}
```

## How to Use
Project Auditor 에디터 윈도우는 *Window => Analysis => Project Auditor* 를 이용하여 열 수 있으며, Analyze 버튼을 클릭하면 다양한 분석 결과를 볼 수 있습니다.
