# 안무서운아이들 (AnMuseoun)

> **They Think They're Scary.**
> 
> 귀엽고 사악한 공포 생명체를 키워서 몰려오는 적을 쓸어버리는 육성 + 뱀서라이크 액션 게임

---

## 게임 소개

**"무서운데 귀엽고, 귀여운데 무섭고, 자꾸 보고 싶은"** 공포 생명체를 키워서 몰려오는 적을 쓸어버리는 게임.

플레이어는 공포 생명체의 조련사로, 크리피큐트한 생명체를 육성하고 웨이브로 몰려오는 적들을 처치합니다. 대충 터치해도 화면에서 축제가 벌어지는, 숫자가 팡팡 터지는 쾌감이 핵심입니다.

### 핵심 컨셉

- **Creepy Cute** — 무서워야 하는데 어딘가 귀여운 캐릭터들
- **숫자 팡팡** — 크리티컬이 터질 때의 시각적 쾌감
- **대충해도 재밌는** — 터치 한 번에 연쇄 반응, 한 판 2분

---

## 기술 스택

| 항목 | 내용 |
|------|------|
| **엔진** | Unity 6 (6000.4.1f1) — URP |
| **언어** | C# |
| **Input** | New Input System |
| **적 AI** | NavMesh (AI Navigation v2.0.11) |
| **애니메이션** | DOTween Pro |
| **데이터** | unity-excel-importer (구글시트 → xlsx → ScriptableObject) |
| **플랫폼** | PC (Steam 예정), 모바일 (포팅 예정) |
| **개발** | 1인 개발 |

---

## 구현된 핵심 시스템

### 데이터 파이프라인

게임 수치를 코드에서 완전히 분리한 데이터 관리 시스템.

```
구글시트 (편집)
    ↓  xlsx 다운로드
Assets/_Project/Data/Excel/GameData.xlsx
    ↓  unity-excel-importer (Editor 전용)
Assets/_Project/Data/Tables/GameData.asset (ScriptableObject)
    ↓  런타임 로드
GameDataManager.Instance.GetEnemyStat(EnemyType.Skeleton)
```

- **IDataSource 인터페이스** — 데이터 소스 교체 지점 (SO → CSV → 서버로 단계별 전환 가능)
- **GameDataManager (Singleton)** — 게임 코드의 유일한 데이터 접근 창구
- **SODataSource** — 현재 구현체 (ScriptableObject 기반)
- 추후 Python/Jenkins 자동화, 서버/Redis 연동 대비 구조

**관리 데이터:**

| 테이블 | 내용 |
|--------|------|
| PlayerStat | 레벨별 HP, ATK, DEF, SPD, 경험치 요구량 |
| EnemyStat | 적 종류별 HP, ATK, DEF, SPD, 경험치 드롭, AI 수치 |
| WeaponStat | 무기별 데미지 배율 |
| WaveStat | 웨이브별 적 종류, 수량, 스폰 간격 |

### FSM (Finite State Machine)

순수 C# 기반 범용 상태 머신. Player, Enemy, Creature 모두 재사용 가능한 구조.

```
IState 인터페이스 (Enter/Update/Exit)
    ↓
StateMachine 클래스 (Dictionary<Type, IState>)
    ↓
PlayerStateMachine / EnemyAI에서 활용
```

- `_Framework/FSM/` — 프로젝트 독립적인 재사용 가능 모듈
- Player 3개 상태: Idle → Move → Attack
- Enemy: if/else 기반 상태 분기 (상태 5개 이상 시 FSM 전환 예정)

### 적 AI (NavMesh 기반)

```
Enemy.cs   — 스탯, HP, 데미지, 사망 (데이터)
EnemyAI.cs — 추적, 분산, 공격 판단 (행동)
```

- **NavMeshAgent** — 장애물 회피, 최단 경로, 적끼리 자동 분산
- **감지 → 추적 → 공격** 상태 분기
- **넉백 코루틴** — 피격 시 밀림 → 경직 → 복귀 (힘 감쇠 곡선 적용)
- 모든 AI 수치 (감지범위, 공격범위, 쿨다운, 이동속도) GameDataManager 연동

### 타격 피드백 시스템

공격 적중 시 3가지 피드백이 동시 발동:

| 피드백 | 구현 |
|--------|------|
| **피격 플래시** | 코루틴으로 모든 MeshRenderer Material → 흰색 → 0.1초 후 복원 |
| **넉백** | NavMeshAgent.velocity 직접 제어, 시간에 따라 힘 감쇠 |
| **카메라 셰이크** | Random.insideUnitSphere + 시간 감쇠, LateUpdate에서 offset 적용 |

### 사운드 관리

```
SoundManager (Singleton)
├── PlayHit()        — 타격음 3종 랜덤 선택
├── PlayEnemyDeath() — 적 사망음
└── PlayBGM()        — BGM (추가 예정)
```

- 중앙 집중식 관리, 개별 오브젝트에 Audio 스크립트 미부착
- `AudioSource.PlayClipAtPoint` — 자동 생성/삭제
- CC0 라이선스 사운드 사용

### 캐릭터 리깅

**Transform 기반 강체 리깅** — SkinnedMeshRenderer 없이 구현.

```
빈 GameObject = 본 (관절)
    └── Cube 메시 = 파츠 (본의 자식)
        → 본이 회전하면 메시가 통째로 따라 회전
```

- MagicaVoxel 모델 → Unity 임포트 예정
- Generic Rig 사용
- 모델 교체 시 게임 로직 코드 수정 불필요 (분리 구조)

---

## 프로젝트 구조

```
Assets/
├── _Framework/              ← 프로젝트 독립, 재사용 가능
│   ├── Core/                Singleton, SoundManager
│   ├── Data/                IDataSource, GameDataManager
│   ├── FSM/                 IState, StateMachine
│   └── Util/                Billboard
│
├── _GamePlay/               ← 이 게임 전용 로직
│   ├── Defines.cs           enum, const 모음
│   ├── Player/              PlayerController, PlayerStateMachine, States/
│   ├── Camera/              CameraController (쿼터뷰)
│   └── Enemy/               Enemy (데이터), EnemyAI (행동)
│
├── _Project/                ← 리소스
│   ├── Animations/          Idle, Walk, Attack 클립
│   ├── Art/Materials/       캐릭터, 환경, 이펙트 Material
│   ├── Audio/SFX/           타격음, 사망음
│   ├── Data/
│   │   ├── Excel/           GameData.xlsx (원본, 에디터 전용)
│   │   ├── Tables/          GameData.asset (ScriptableObject)
│   │   └── SODataSource.cs  데이터 소스 구현체
│   ├── Prefabs/             캐릭터, 적, 이펙트 프리팹
│   └── Scenes/              메인 씬
│
└── Plugins/                 DOTween Pro
```

---

## 밸런스 수치

### 플레이어

| 레벨 | HP | ATK | DEF | SPD | 필요 EXP |
|------|----|-----|-----|-----|---------|
| 1 | 100 | 10 | 5 | 5.0 | 100 |
| 2 | 120 | 13 | 7 | 5.4 | 283 |
| 3 | 140 | 16 | 9 | 5.8 | 520 |
| 4 | 160 | 19 | 11 | 6.2 | 800 |
| 5 | 180 | 22 | 13 | 6.6 | 1150 |

### 적

| 이름 | HP | ATK | SPD | EXP 드롭 |
|------|----|-----|-----|---------|
| Skeleton | 50 | 8 | 3.0 | 10 |
| FastZombie | 30 | 6 | 5.0 | 15 |
| TankZombie | 150 | 12 | 2.0 | 25 |

### 무기

| 무기 | 데미지 배율 |
|------|-----------|
| Punch | × 1.0 |
| Axe | × 1.8 |

---

## 현재 개발 상태

### 완료

- [x] 쿼터뷰 카메라 (디아블로 스타일, Lerp 추적)
- [x] 플레이어 이동 (우클릭) + 공격 (좌클릭, 방향 전환)
- [x] CharacterController 기반 이동 + 벽 충돌
- [x] FSM 상태 머신 (Idle/Move/Attack)
- [x] 복셀 해골 적 캐릭터 (본 계층 + 리깅 + Walk 애니메이션)
- [x] NavMesh 기반 적 AI (추적 + 공격)
- [x] 적 HP 시스템 (Quad 기반 빌보드 HP바)
- [x] 플레이어 HP 시스템 (Screen Space UI)
- [x] 타격 피드백 (피격 플래시 + 넉백 + 카메라 셰이크)
- [x] 공격 이펙트 (트레일 + 손끝 불꽃 + 번개 관통)
- [x] 사운드 (SoundManager, 타격음 3종 + 사망음)
- [x] 사망 연출 (연기 파티클 + Destroy)
- [x] 게임오버 (흑백 처리 + GAME OVER UI + 씬 리로드)
- [x] 어두운 고딕 환경 (붉은 달, 안개, 네온, Post Processing)
- [x] 데이터 파이프라인 (구글시트 → xlsx → SO → GameDataManager)
- [x] 전투 수치 데이터화 (플레이어/적/무기/웨이브 전 수치 분리)

### 개발 예정

- [ ] DamageCalculator (무기배율 + 크리티컬 공식)
- [ ] 데미지 숫자 팝업 (DOTween 연출)
- [ ] 크리티컬 + 콤보 시스템
- [ ] ObjectPool (적 대량 스폰 대비)
- [ ] 레벨 / 경험치 시스템
- [ ] 웨이브 시스템 (적 리스폰 + 단계별 난이도)
- [ ] 맵 진행 구조 (위→아래, 되돌아갈 수 없음)
- [ ] SOUL 재화 시스템
- [ ] 캐릭터 MagicaVoxel 모델 교체
- [ ] BGM

---

## 개발 원칙

1. **플레이 재미가 먼저** — 코어 루프가 재미있어야 시스템이 의미 있다
2. **대충해도 재밌는 게임** — 터치 한 번에 화면에서 축제가 벌어지는 게임
3. **바이브코딩 금지** — 이해 못한 코드는 붙여넣지 않는다
4. **완벽한 코드보다 돌아가는 코드** — 2주마다 눈에 보이는 결과물
5. **Top-Down 코딩** — 의도가 먼저, 구현이 나중

---

## 실행 방법

1. Unity Hub에서 **Unity 6 (6000.4.1f1)** 설치
2. 이 리포지토리 클론
3. Unity Hub에서 프로젝트 열기
4. `_Project/Scenes/SampleScene.unity` 열기
5. ▶ Play

### 조작법

| 입력 | 동작 |
|------|------|
| 우클릭 | 클릭 지점으로 이동 |
| 좌클릭 | 클릭 방향으로 회전 후 공격 |

---

## 라이선스

이 프로젝트는 개인 포트폴리오용입니다.

- 코드: 개인 저작물
- 사운드: CC0 라이선스 (freesound.org)
- 캐릭터 모델: 직접 제작 (플레이스홀더)

---

*안무서운아이들 | They Think They're Scary. | 2026 | 1인 인디 개발*
