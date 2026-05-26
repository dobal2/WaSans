# WaSans

언더테일 샌즈전에서 영감을 받은 2D 픽셀아트 액션 플랫포머 보스러시 게임입니다.
스테이지를 클리어하며 다양한 적을 처리하고, 최종 보스 Sans와 전투합니다.

## 플레이 영상

[![WaSans 플레이 영상]([https://img.youtube.com/vi/XL9qDXp5k9s/0.jpg)](https://www.youtube.com/watch?v=XL9qDXp5k9s](https://youtu.be/uNmG5V911ko))

---

## 개요

| 항목 | 내용 |
|---|---|
| 엔진 | Unity |
| 언어 | C# |
| 개발 인원 | 1인 개발 |
| 개발 기간 | 2025.04 ~ 2025.04 (약 2주) |
| 해상도 | 3840×2160 타겟, 적응형 스케일링 |

---

## 게임 플레이

```
Stage 1 → Stage 2 → Stage 3 → Sans 보스전 → 클리어
```

- 각 스테이지의 적을 모두 처리하면 다음 스테이지로 진행
- 스테이지 간 체력 회복
- 사망 시 현재 스테이지에서 재시작

### 조작법

| 키 | 동작 |
|---|---|
| `A/D` 또는 `←/→` | 좌우 이동 |
| `Space` | 점프 (공중에서 더블 점프) |
| `좌클릭` | 근접 공격 (2히트 콤보) |
| `우클릭` | 대시 |

---

## 주요 기술

### 패턴형 보스 AI — 거리 기반 행동 분기

일반 적은 플레이어와의 거리를 기준으로 근접 공격 / 대기 / 추격을 분기합니다.
Sans 보스는 거리가 멀어지면 텔레포트로 즉시 접근해 전투의 템포와 긴장감을 끊김 없이 유지합니다.
Coroutine과 상태 플래그로 패턴 간 충돌을 방지해, 9가지 공격 패턴이 독립적으로 실행됩니다.

| 패턴 | 설명 |
|---|---|
| Bone | 단일 뼈 투사체 |
| MultiBone | 일반 / 파랑 / 주황 뼈 5개 랜덤 발사 |
| Teleport | 플레이어 뒤로 순간이동 |
| BlueBone | 뼈 10개 — 가만히 있으면 회피 가능 |
| OrangeBone | 뼈 10개 — 움직이면 회피 가능 |
| Blue & Orange | 혼합 뼈 10개, 고속 |
| Gaster Blaster | 단일 빔 |
| Triple Gaster Blaster | 3연 빔 |
| Tracking Gaster Blaster | 플레이어 위치 추적 빔 |

### 아웃라인 셰이더 — 전투 가독성 강화

플레이어와 적 캐릭터에 아웃라인 셰이더를 적용해, 배경과 겹치는 상황에서도 위치를 즉시 파악할 수 있습니다.
피격 및 상태 변화 시 아웃라인 색상이 바뀌어 별도의 UI 없이 시각적 피드백을 전달합니다.

### 패럴랙스 배경 + 카메라 컬링

레이어별 속도 차이를 주는 패럴랙스 배경으로 2D 플랫포머에 깊이감을 더했습니다.
카메라 밖으로 벗어난 오브젝트를 정리해 불필요한 메모리 점유를 줄였습니다.

### 대시 잔상 이펙트

대시 중 일정 간격마다 플레이어 스프라이트를 복제해 잔상 오브젝트를 생성합니다.
알파값을 프레임마다 감소시켜 페이드아웃 처리하고, 완전히 투명해지면 자동 파괴합니다.
대시 무적 프레임과 맞물려 시각적으로 회피 타이밍을 직관적으로 전달합니다.

### 스테이지 관리 — GameManager

GameManager가 스테이지 진행, 적 스폰, 클리어 판정을 단일 지점에서 관리합니다.
모든 적 처치 감지 후 자동으로 다음 스테이지로 전환되며, 스테이지별 적 배치를 데이터로 분리해 확장에 용이합니다.

### Tank 포물선 사격

발사 각도 70도를 기준으로 플레이어 위치까지의 탄도를 역산해 포물선 궤적으로 발사합니다.
착탄 시 폭발 이펙트가 발생해 범위 공격임을 시각적으로 명확히 전달합니다.

---

## 기술 사항

| 항목 | 내용 |
|---|---|
| 디자인 패턴 | Singleton (GameManager, AudioManager) |
| 이펙트 | 대시 잔상 · 카메라 셰이크 · 플로팅 데미지 텍스트 · 피격 무적 + 넉백 |
| 오디오 | BGM / SFX 분리 관리, 채널 기반 다중 SFX 동시 재생 |

---

## 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── CharacterController2D.cs   # 플레이어 물리/이동
│   │   ├── PlayerMovement.cs          # 입력 처리
│   │   ├── Attack.cs                  # 콤보 공격
│   │   ├── CameraFollow.cs            # 카메라 추적/셰이크
│   │   ├── HealthDisplay.cs           # 하트 HP UI
│   │   └── DashAfterImage.cs          # 대시 잔상 이펙트
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
│   ├── MainGame.unity
│   └── Clear.unity
├── Prefabs/
├── Sprites/
└── Audio/
```

---

## 사용 에셋

| 에셋 | 용도 |
|---|---|
| Prototype Hero - Pixel Art | 플레이어 캐릭터 스프라이트 |
| Bandits - Pixel Art | 적 캐릭터 스프라이트 |
| Pixel Art Tanks | Tank 적 스프라이트 |
| Industrial City | 배경 타일맵 |
| 19 Pixel Sprite effects | 이펙트 |
| 25 Pixel Sprite effects | 이펙트 |
