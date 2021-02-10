# About Project Auditor
Project Auditor 는 
Project Auditor 는 유니티 프로젝트를 정적으로 분석해주는 툴입니다. Project Auditor 는 스크립트와 세팅을 분석하여 성능에 문제가 될만한 부분을 찾아줍니다.

Project Auditor 패키지를 사용하여 스크립트와 Project Setting 을 분석할 수 있으며, 기존에 문제가 되었던 부분들을 모아서 보여줍니다. 이후에 사용자가 직접 실제로 문제가 되는 부분인지 아닌 지 확인할 수 있습니다.

## Preview package
이 패키지는 프리뷰 버전 이며, 기능과 문서는 변경될 수 있습니다.

# Installing Project Auditor
Project Auditor 는 Unity 2018 버전부터 패키지로 설치할 수 있습니다.

Add `com.unity.project-auditor` as a dependency to the project `Packages/manifest.json` file:

```
{
  "dependencies": {
    "com.unity.project-auditor": "https://github.com/Unity-Technologies/ProjectAuditor.git",
  }
}
```

# Using Project Auditor
Project Auditor 에디터 윈도우는 Window => Analysis => Project Auditor 를 이용하여 열 수 있으며

<img src="images/window-menu.png">

Auditor 윈도우가 열리면 *Analyze* 버튼을 눌러 프로젝트 분석을 시작합니다.

<img src="images/intro.png">

분석은 프로젝트 크기에 따라 보통 몇초 정도 걸립니다. 분석이 끝나면  Project Auditor 는 잠재적인 문제 사항들을 리포트합니다.

<img src="images/overview.png">

해당 이슈들은 코드인지 프로젝트 세팅인지에 따라 탭으로 구분되어있으며, 해당 탭은 변경 가능합니다.

<img src="images/category.png">

필터를 이용하여 String, Assembly, 또는 다른 특성을 이용하여 검색할 수 있습니다.

<img src="images/filters.png">

이슈들은 테이블로 표시되며, 테이블에서는 문제 영역, 파일명 등을 볼 수 있습니다. 
The issues are displayed in a table containing some details regarding impacted area, filename, etc.

<img src="images/issues.png">

오른쪽 윈도우에서는 선택한 문제에 대해 추가적이 사항을 확인할 수 있습니다. 맨 위의 창에서는 해당 문제에 대한 제사한 설명을 확인할 수 있으며, 바로 아래에서 추천하는 솔루션이 있습니다. 맨 아래에는 현재 선택한 함수의 콜스택을 볼 수 있습니다.

<img src="images/panels.png">

Mute/Unmute 버턴을 이용하여 특정 이슈(그룹)을 무시할 수 있습니다.

<img src="images/mute.png">

## Running from command line

Project Auditor 는 editor 를 배치모드로 실행 후, command line으로 실행 할 수 있습니다.
이 때 Editor Script 가 필요한 데, 아래와 같은 코드가 필요합니다.

* ProjectAuditor object 생성
* 분석 실행
* 리포트 내보내기

예제:

```
using Unity.ProjectAuditor.Editor;
using UnityEngine;

public static class ProjectAuditorCI
{
    public static void AuditAndExport()
    {
        var configFilename = "Assets/Editor/ProjectAuditorConfig.asset";
        var outputFilename = "project-auditor-report.csv";

        var projectAuditor = new ProjectAuditor(configFilename);
        var projectReport = projectAuditor.Audit();
        projectReport.ExportToCSV(outputFilename);
    }
}
```
Unity를 command line으로 실행하기 위한 좀 더 자세한 정보는 
아래 문서에서 확인 가능합니다.
[manual](https://docs.unity3d.com/kr/current/Manual/CommandLineArguments.html).

# Technical details
## Requirements
현재 버전의 Project Auditor는 아래 Unity 버전이 필요합니다.

* 5.6 and later. However, Unity 2018 is required to install Project Auditor as a package.

## 알려진 이슈
Project Auditor 사용 시 알려진 이슈입니다.

* 빌드 과정중에 strip 될 코드들에 대해서도 문제를 리포트 합니다.
* Cold 와 Hot path 를 구분할 수 없습니다.
* Call tree 분석의 경우 가상함수를 지원하지 않습니다.

## Package contents
패키지 구조는 아래와 같습니다.:

|Location|Description|
|---|---|
|`Data`|Contains the issue definition database.|
|`Documentation~`|Contains documentation files.|
|`Editor`|Contains all editor scripts: Project Auditor and external DLLs.|
|`Editor/UI`|Project Auditor Editor window.|
|`Tests`|Contains all scripts required to test the package.|

## Document revision history
|Date|Reason|
|---|---|
|Oct 16, 2020|Added information about command line execution|
|May 21, 2020 |Expanded *Using Project Auditor* section|
|Dec 4, 2019|First draft.|
