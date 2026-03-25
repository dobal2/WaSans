# WaSans

Undertale의 Sans 전투에서 영감을 받은 2D 픽셀아트 액션 플랫포머 보스러시 게임입니다.
스테이지를 클리어하며 다양한 적을 처리하고, 최종 보스 Sans와 전투합니다.

## 게임 플레이

```
Stage 1 → Stage 2 → Stage 3 → Sans 보스전 → 클리어
```

- 각 스테이지의 적을 모두 처리하면 다음 스테이지로 진행됩니다
- 스테이지 간 체력이 회복됩니다
- 사망 시 현재 스테이지에서 재시작합니다

### 조작법

| 키 | 동작 |
|---|---|
| `A/D` 또는 `←/→` | 좌우 이동 |
| `Space` | 점프 (공중에서 더블 점프) |
| `좌클릭` | 근접 공격 (2히트 콤보) |
| `우클릭` | 대시 |
| `G` | 투척 무기 |

---

## 핵심 시스템

### 플레이어 컨트롤러
- 부드러운 가감속 이동과 에어 컨트롤을 지원합니다
- 지상 점프, 더블 점프, 벽 슬라이딩, 벽 점프가 가능합니다
- 쿨다운 기반 대시 시 잔상 이펙트가 생성됩니다
- 피격 시 무적 프레임, 넉백, 스턴이 적용됩니다

### 콤보 공격
- 2히트 근접 콤보로 원형 범위 충돌 검출 방식입니다
- 투척 무기를 통한 원거리 공격이 가능합니다

### Sans 보스 AI — 9가지 공격 패턴

| 패턴 | 설명 |
|---|---|
| Bone | 단일 뼈 투사체 |
| MultiBone | 일반/파랑/주황 뼈 5개 랜덤 발사 |
| Teleport | 플레이어 뒤로 순간이동 |
| BlueBone | 파란 뼈 10개, 가만히 있으면 회피 가능 |
| OrangeBone | 주황 뼈 10개, 움직이면 회피 가능 |
| Blue & Orange | 혼합 뼈 10개, 고속 |
| Gaster Blaster | 단일 빔 공격 |
| Triple Gaster Blaster | 3연 빔 공격 |
| Tracking Gaster Blaster | 플레이어 위치 추적 빔 |

거리에 따라 걷기와 순간이동을 전환하는 이동 AI가 적용되어 있습니다.

### 특수 투사체 메카닉
- BlueBone — 움직이는 플레이어만 피격되며, 멈추면 회피할 수 있습니다
- OrangeBone — 정지한 플레이어만 피격되며, 움직이면 회피할 수 있습니다

### 적 유형

| 적 | 특징 |
|---|---|
| Enemy | 좌우 순찰, 접촉 데미지 |
| Tank | 탄도 계산 기반 포물선 사격 |
| Ally | 의사결정 기반 아군 AI |
| Sans | 최종 보스, 9종 패턴 |

---

## 기술 스택

| 항목 | 내용 |
|---|---|
| 엔진 | Unity |
| 언어 | C# |
| 해상도 | 3840×2160 타겟, 적응형 스케일링 |

### 디자인 패턴
- Singleton — GameManager, AudioManager를 씬 간 상태 유지에 사용했습니다
- 상속 — Enemy 기반 클래스에서 적 유형을 확장했습니다
- Coroutine — 타이밍 및 상태 전환 관리에 활용했습니다

### 이펙트
- 대시 잔상 (스프라이트 복제 + 알파 페이드)
- 카메라 셰이크
- 플로팅 데미지/Miss 텍스트
- 피격 무적 프레임, 넉백
- 인터랙티브 풀, 파괴 가능 오브젝트

### 오디오
- BGM과 SFX를 분리하여 관리합니다
- 채널 기반 다중 SFX 동시 재생을 지원합니다

---

## 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── CharacterController2D.cs   # 플레이어 물리/이동
│   │   ├── PlayerMovement.cs          # 입력 처리
│   │   ├── Attack.cs                  # 콤보 공격/투척
│   │   ├── CameraFollow.cs            # 카메라 추적/셰이크
│   │   ├── HealthDisplay.cs           # 하트 HP UI
│   │   ├── DashAfterImage.cs          # 대시 잔상 이펙트
│   │   └── ThrowableWeapon.cs         # 투척 무기
│   ├── Enemies/
│   │   ├── Sans.cs                    # 보스 AI (9가지 패턴)
│   │   ├── GasterBlaster.cs           # 보스 빔 공격
│   │   ├── Enemy.cs                   # 기본 적 AI
│   │   ├── Tank.cs                    # 포탑 적
│   │   ├── Ally.cs                    # 아군 AI
│   │   ├── Bone.cs                    # 기본 투사체
│   │   ├── BlueBone.cs                # 파란 뼈 (멈추면 회피)
│   │   └── OrangeBone.cs             # 주황 뼈 (움직이면 회피)
│   ├── GameManager.cs                 # 스테이지/게임 상태 관리
│   ├── AudioManager.cs                # 오디오 시스템
│   └── Environment/                   # 풀, 파괴 오브젝트, 킬존
├── Scenes/
│   ├── MainGame.unity                 # 메인 게임 씬
│   └── Clear.unity                    # 클리어 씬
├── Prefabs/
├── Sprites/
└── Audio/
```

---

## 실행 방법

1. Unity Hub에서 프로젝트를 열어주세요
2. `Assets/Scenes/MainGame.unity` 씬을 열고 Play 버튼을 클릭하시면 됩니다
