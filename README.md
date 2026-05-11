# Idle Farm — Cập nhật sau Feedback

---

## Các vấn đề được chỉ ra

### 1. Tiền tệ lớn sử dụng BigNumber bị ép kiểu về double
**Trước:** Dùng `Math.Pow(10, exponent)` để tính toán — ép kiểu về `double`, mất độ chính xác ở số rất lớn.

**Sau:** Thay toàn bộ bằng vòng lặp nhân/chia 10 thuần túy, không qua `Math.Pow`:
```csharp
// Thay vì: mantissa * Math.Pow(10, exp)
if (exp > 0)
    for (int i = 0; i < exp; i++) mantissa /= 10.0;
else
    for (int i = 0; i < -exp; i++) mantissa *= 10.0;
```

---

### 2. Xử lý hiển thị hậu tố lặp ở nhiều nơi
**Trước:** Mỗi class (popup, UI, slot) đều có hàm `FormatNumber()` riêng — lặp code, khó bảo trì.

**Sau:** Tập trung vào `BigNumberFormatter` duy nhất. `BigNumber.ToString()` tự gọi formatter:
```csharp
public static class BigNumberFormatter
{
    private static readonly (int exponent, string suffix)[] Suffixes =
    {
        (15, "Q"), (12, "T"), (9, "B"), (6, "M"), (3, "k"),
    };
    public static string Format(BigNumber number) { ... }
}
```
Toàn bộ codebase chỉ gọi `value.ToString()` — không còn `FormatNumber` nào khác.

---

### 3. ScriptableObject Upgrade handle còn hạn chế (switch case cứng)
**Trước:** `UpgradeOptionView` dùng `switch (config.upgradeType)` — thêm loại upgrade mới phải sửa code.

**Sau:** Dùng `IUpgradeAction` interface — mỗi loại upgrade là 1 ScriptableObject riêng, không cần switch:
```csharp
public interface IUpgradeAction
{
    void Execute(GameController gameController);
}

// Thêm loại upgrade mới: chỉ cần tạo ScriptableObject mới implement IUpgradeAction
// Không sửa bất kỳ code hiện có nào
```

---

### 4. Chưa xử lý chỉ số thay đổi từ nhiều nguồn (+, +%, x)
**Trước:** Profit chỉ được nhân trực tiếp `_profit *= multiplier` — không phân biệt nguồn, không thể remove buff.

**Sau:** `StatValue` + `StatModifier` xử lý đúng thứ tự ưu tiên:
```
Flat (+)      → áp dụng trước
Percent (+%)  → cộng tổng % rồi nhân 1 lần
Multiply (x)  → nhân từng cái sau cùng
```
```csharp
// Ví dụ: base = 100, +50 flat, +20%, x2
// Kết quả: (100 + 50) × 1.2 × 2 = 360

_profitStat.AddModifier(new StatModifier(ModifierType.Flat,     50,   "flat_source"));
_profitStat.AddModifier(new StatModifier(ModifierType.Percent,  0.2,  "upgrade_lv2"));
_profitStat.AddModifier(new StatModifier(ModifierType.Multiply, 2.0,  "x2_global"));
```
Mỗi modifier có `Source` — có thể remove theo nguồn bất kỳ lúc nào.

---

### 5. Vi phạm Single Responsibility (S trong SOLID)
**Trước:** `BaseSlot`, `BaseCustomer`, `BaseWorker` mỗi class làm quá nhiều việc: quản lý state, xử lý animation, tính toán data, gọi UI, gọi Currency.

**Sau:** Tách thành các class nhỏ, mỗi class 1 trách nhiệm:

| Class cũ | Tách thành |
|---|---|
| `BaseSlot` | `SlotData` (data) + `SlotStateMachine` (state) + `SlotView` (visual) + `BaseSlot` (phối hợp) |
| `BaseCustomer` | `CustomerData` + `CustomerStateMachine` + `CustomerAnimatorController` + `BaseCustomer` (phối hợp) |
| `BaseWorker` | `WorkerStateMachine` + `WorkerAnimatorController` + `BaseWorker` (phối hợp) |
| `BaseSlot` click logic | `SlotClickHandler` (static class riêng) |

---

### 6. Vi phạm Interface Segregation (I trong SOLID)
**Trước:** `ISlot`, `ICustomer`, `IWorker` quá lớn — class implement bị ép implement những method không liên quan.

**Sau:** Tách thành các interface nhỏ, đúng mục đích:

```csharp
IInitializable  → OnInit()
IUpdatable      → OnUpdate()
IStateful<T>    → GetState(), ChangeState()
IHarvestable    → OnHarvest(), IsReady
IUnlockable     → TryUnlock(), IsUnlocked
IUpgradeable    → TryUpgrade(), GetUpgradeCost(), GetNextProfit(), Level
IBuffable       → ApplyModifier()
IMovable        → WarpTo(), GetStandPoint()
IDispatchable   → OnGenerateOrder(), GetWorkerState()
```

`BaseSlot` implement đúng những interface cần thiết:
```csharp
public class BaseSlot : MonoBehaviour,
    IInitializable, IUpdatable,
    IHarvestable, IUnlockable, IUpgradeable, IBuffable
```

---

### 7. Lạm dụng Singleton
**Trước:** `CurrencyManager.Instance`, `PopupManager.Instance` được gọi trực tiếp từ khắp nơi — khó test, khó thay thế.

**Sau:** Singleton duy nhất là `GameController`. Tất cả dependency được inject qua `OnInit`:

```
GameController.Instance (Singleton duy nhất)
    ├── CurrencyManager  ← inject qua SlotController.OnInit, WorkerController.OnInit
    ├── PopupManager     ← inject qua SlotController.OnInit, BaseSlot.Setup
    ├── SlotController
    ├── WorkerController
    └── CustomerController
```

`IUpgradeAction.Execute(GameController gc)` — action nhận `GameController` qua tham số thay vì gọi `Instance` trực tiếp.

---

## Cấu trúc sau khi refactor

```
Core/
  Interfaces.cs         ← 9 interface nhỏ
  StatValue.cs          ← StatModifier + StatValue
  GameController.cs     ← Singleton duy nhất

Currency/
  BigNumber.cs          ← tính toán không dùng Math.Pow
  BigNumberFormatter.cs ← format tập trung duy nhất
  CurrencyManager.cs    ← không Singleton

Slot/
  SlotCore.cs           ← SlotConfig + SlotData + SlotStateMachine
  SlotView.cs           ← SlotView + SlotWorldUI + SlotClickHandler
  BaseSlot.cs           ← phối hợp, implement interfaces
  SlotController.cs

Customer/
  CustomerCore.cs       ← CustomerData + CustomerStateMachine + CustomerAnimatorController
  BaseCustomer.cs       ← phối hợp
  CounterSlotManager.cs
  CustomerController.cs

Worker/
  WorkerCore.cs         ← WorkerState + OrderDat + WorkerStateMachine + WorkerAnimatorController
  BaseWorker.cs         ← phối hợp
  WorkerController.cs

Upgrade/
  UpgradeActions.cs     ← IUpgradeAction + 4 implementations (không switch)
  UpgradeOptionConfig.cs

UI/
  Popup/
    BasePopupAndManager.cs
    Popups.cs
  UIScripts.cs
```
