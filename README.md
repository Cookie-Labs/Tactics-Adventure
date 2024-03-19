# Tatics Adventure

<details>
<summary>Alpha 1.2</summary>
**[Alpha 1.2]**<br/>
- 스테이지별 몬스터 해금 활성화<br/>
&nbsp; &nbsp; &nbsp; &nbsp;Grass -> Goblin, Slime, Mushroom<br/>
&nbsp; &nbsp; &nbsp; &nbsp;Cave -> Skeleton, Ghost, Poison Soul<br/>
&nbsp; &nbsp; &nbsp; &nbsp;Swarm -> Fire Soul, Ice Soul, Monkey
- Empty Card 새로 등장<br/>
- Card 이미지 스프라이트 추가 (빈카드, 보스카드)<br/>
- 설명 추출 로직 추가 (Chest, Empty에 적용) (Player 추가 예정)<br/>

**[Alpha 1.2.1]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/286abc1d-665e-4224-80f8-344b3349d3a2" width="540" alt="Alpha 1.2.1">
<br/>
- 스킬 버튼 추가<br/>
- 스킬 아이콘 스프라이트 Import<br/>
- Canvas 추가<br/>
- 무기 데미지 산정 공식 재정의<br/>
- 카드 애니메이션(닷트윈) 추가 (스폰, 삭제, 데미지)<br/>

**[Alpha 1.2.2]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/1b58c2ea-4ea6-4e22-98cd-25b6fac02c1a" width="540" alt="Alpha 1.2.2">
<br/>
- Knight 스킬 추가<br/>
&nbsp; &nbsp; &nbsp; &nbsp;Active : 무기 공격력 1 증가, 무기가 없다면 공격력 1인 롱소드 장착<br/>
&nbsp; &nbsp; &nbsp; &nbsp;Passive : 방어도 1 증가
- 스킬 UI 쿨타임 표시<br/>
- 턴 알고리즘 최적화<br/>
- UIManager, BtnManager 추가<br/>
- 터치 이벤트 알고리즘 재정의<br/>
- 돈 UI 추가<br/>
- 데미지 받았을 때 텍스트 나오지 않던 버그들 수정<br/>
- MP, Defend UI와 알고리즘 추가<br/>
- Player 무기 장착 알고리즘 변경<br/>

**[Alpha 1.2.3]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/e30bb835-1f3d-4059-829b-8eca463e647f" width="540" alt="Alpha 1.2.3">
<br/>
- Player 애니메이션 추가 (Idle, Walk, Damaged, Atk, Die, Interaction)<br/>
- Monster 애니메이션 추가 (Idle, Walk, Damaged, Atk, Die)<br/>
- 딜레이 로직 변경 (0.1f -> 애니메이션 종료 시점)<br/>
- 중복된 이미지 통합<br/>
- 카드 작동 관련 전부 코루틴화<br/>
- 카드 작동 중 액티브 스킬 비활성화<br/>

**[Alpha 1.2.4]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/e3c7ed1d-1b97-4cec-b4f3-2c8016826d36" width="540" alt="Alpha 1.2.4">
<br/>
- 손 이미지 추가 (활성화, 비활성화)<br/>
- 손 시스템 추가<br/>
&nbsp; &nbsp; &nbsp; &nbsp;이제 무기를 최대 2개 저장할 수 있습니다.<br/>
&nbsp; &nbsp; &nbsp; &nbsp;손 UI가 플레이어의 클릭에 반응합니다.<br/>
&nbsp; &nbsp; &nbsp; &nbsp;터치 동작 수행중이 아니라면 언제든지 손을 변경할 수 있습니다.
- UI 재배치<br/>

**[Alpha 1.2.5]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/2ffcf7a0-02fc-4675-99fc-29f8edf87e79" width="540" alt="Alpha 1.2.5">
<br/>
- Player 말 기능 추가 (피격, 공격, 클릭)<br/>
- Card 비활성화 스프라이트 추가<br/>
- 클릭 불가 카드 비활성화 시각화<br/>
- 사용 중인 카드 포커스화<br/>

**[Alpha 1.2.6]**<br/>
<img src="https://github.com/HongDaHyun/Tactics-Adventure/assets/101586627/18bbc0ba-ba60-41dc-9e64-0b93c158fcd6" width="540" alt="Alpha 1.2.6">
<br/>
- 버그 수정<br/>
&nbsp; &nbsp; &nbsp; &nbsp;트랩 공격 딜레이 비정상<br/>
&nbsp; &nbsp; &nbsp; &nbsp;몬스터 데미지 관련 코루틴 호출 문제<br/>
&nbsp; &nbsp; &nbsp; &nbsp;플레이어 주변 카드 때때로 찾지 못하는 버그<br/>
- 가방 UI 이미지 스프라이트 추가<br/>
- 가방 추가 (유물 보관용)<br/>
- 유물 아이콘 UI 프리팹화<br/>

**[Alpha 1.2.7]**<br/>
- 유물 제작(30%)<br/>
- 운 요소 등장<br/>
- 무기 속성 추가(물리, 마법)<br/>
- 돈 시스템 재정의<br/>
- 잘못된 유물 설명 수정<br/>
- EquipWeapon Struct 삭제 -> WeaponData Struct로 통일<br/>
- 에디터 프리징(무한로딩) 현상 해결<br/>
- 무기 추가 스탯 구조체 추가(WeaponPlus)<br/>
- 액티브 스킬 사용 시 스킬UI 무반응 버그 수정<br/>
- 기본 공격력(보너스 공격력) 추가<br/>
- 기타 스크립트 최적화<br/>
</details>

<details>
<summary>Alpha 1.3</summary>
**[Alpha 1.3]**<br/>
<b>[추가 사항]</b>
- 무기 타입 (물리, 마법)<br/>
- 빈 손 & 장착한 무기 알아내는 알고리즘 추가<br/>
- 독(디버프) 추가<br/>
- 무기 속성 추가(생명력 흡수...)<br/>
- 스탯 추가 (공격력, 회복력, 약화 등등)<br/>
- 무적 추가<br/>
- 이펙트 추가 (무적 이펙트)<br/>
- 운 추가<br/>
- <b>유물 완성!!!!</b><br/>
- 부활 추가<br/>
- 카드 위치 셔플&교환 기능 추가, 카드 복제 기능 추가<br/>
- 유물 삭제&수집 기능 추가<br/>
- 공짜 스킬 추가<br/>
- 턴 유물 기능 추가<br/>
<br/>
<b>[교체 사항]</b>
- 플레이어 카드 UI 재정립<br/>
- 돈 스크립트 변경<br/>
<br/>
<b>[버그 수정]</b>
- 전사 액티브 스킬 버그 수정<br/>
- 손 UI 버그 수정<br/>
- 무기 데이터 불러올 때 버그 수정<br/>
</details>