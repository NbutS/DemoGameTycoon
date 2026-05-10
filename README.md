# Idle Farm — Unity Demo
**Unity 2021.3.x LTS**

---

## Mô tả

Mô phỏng hoạt động của một nông trại bán hoa quả theo thể loại Idle Tycoon. Người chơi quản lý các ô đất trồng cây, điều phối nhân viên thu hoạch và giao hàng cho khách hàng để kiếm tiền, sau đó dùng tiền để mở rộng và nâng cấp nông trại.

---

## Tính năng đã hoàn thành

### Xây dựng
- Click vào hộp gỗ → hiện popup Unlock với giá mở khóa
- Trừ tiền → animation hộp mở → cây trồng xuất hiện với animation mọc lên
- Mỗi slot có `SlotConfig` (ScriptableObject) cấu hình độc lập: giá unlock, profit cơ bản, thời gian cooldown

### Thu hoạch & Giao hàng
- Worker tự động phát hiện cây có quả → di chuyển đến thu hoạch → mang về quầy bán
- Tiền chỉ được cộng **sau khi giao hàng thành công**, không phải lúc thu hoạch
- Worker ưu tiên đến slot có **profit cao nhất** trước
- Hỗ trợ nhiều Worker hoạt động đồng thời, không xảy ra tình trạng 2 Worker nhận cùng 1 đơn hàng

### Mua hàng
- Customer tự động đi ra quầy → đứng chờ tại vị trí được phân công → nhận hàng → rời đi → lặp lại
- Ban đầu có 1 Customer, có thể mở rộng tối đa 4
- Mỗi Customer có vị trí đứng riêng tại quầy, không chồng chéo nhau

### Nâng cấp Cây trồng
- Click vào cây đã unlock → hiện popup Upgrade
- Hiển thị: level hiện tại, profit hiện tại, profit sau khi nâng cấp, giá nâng cấp
- Mỗi lần nâng cấp tăng profit theo hệ số cấu hình trong `SlotConfig`

### Quản lý & Upgrade chung
- **X2 Profit từng slot**: nhân đôi profit của 1 cây trồng cụ thể
- **X2 Global Profit**: nhân đôi profit của tất cả cây trồng
- **Thêm Customer**: tăng số lượng khách hàng tối đa
- **Thêm Worker**: tăng số lượng nhân viên tối đa
- Giá mỗi option tự động tăng gấp đôi sau mỗi lần mua

### Di chuyển
- Sử dụng **NavMesh + NavMeshAgent** để tìm đường ngắn nhất
- Tự động tránh vật cản (hộp gỗ, các slot)
- Worker và Customer tự xoay mặt theo hướng di chuyển
- Customer xoay mặt về phía quầy khi đứng chờ

### Tiền tệ
- Sử dụng **`BigNumber` struct** tự xây dựng để xử lý số nguyên lớn chính xác
- Hiển thị định dạng: `k`, `M`, `B`, `T`, ký hiệu khoa học khi vượt nghìn tỷ
- Toàn bộ ScriptableObject là **read-only** từ code, tránh tình trạng giá trị bị ghi đè khi build APK

### UI
- **Top HUD**: hiển thị coin realtime, tự cập nhật khi số tiền thay đổi
- **World Space UI** trên mỗi slot: icon + profit hiện tại, luôn quay về camera
- **Popup System** có thể mở rộng: `BasePopup` → các popup kế thừa
  - `UnlockSlotPopup`: hiện đúng vị trí trên đầu slot được click
  - `UpgradeSlotPopup`: hiện đúng vị trí trên đầu slot được click
  - `GlobalUpgradePopup`: hiện toàn màn hình
  - `NotEnoughCoinPopup`: tự động đóng sau 1.5 giây
- Popup không bị trigger khi click vào UI button phía trên

---

## Kiến trúc hệ thống

```
GameController
  ├── SlotController      — quản lý list slot, fire event khi slot Ready
  ├── CustomerController  — quản lý list customer, fire event khi customer Waiting
  └── WorkerController    — quản lý list worker, dispatch khi có event

GameController là nơi duy nhất quyết định dispatch Worker
Các Controller không biết nhau, giao tiếp qua event
```

### Pattern sử dụng
- **Event-driven**: SlotController → GameController → WorkerController, không polling trong Update
- **State Machine**: Worker (`Idle → MovingToSlot → Harvesting → MovingToCustomer → Delivering`) và Customer (`Idle → MovingToCounter → Waiting → ReceiveGood → MoveOut`)
- **ScriptableObject**: cấu hình dữ liệu cho Slot và Upgrade Option, tách biệt hoàn toàn với logic
- **Popup Manager**: quản lý tập trung, thêm popup mới chỉ cần tạo class kế thừa `BasePopup`
- **Service Locator** qua Singleton: `CurrencyManager`, `PopupManager`, `GameController`

---

## Những điểm kỹ thuật nổi bật

**BigNumber** — xử lý số lớn chính xác thay vì dùng `double` thuần:
```
2000 → 2k → 1.5M → 3.2B → 1.7T → 2.5e15
```

**Ưu tiên slot profit cao nhất** — Worker luôn chọn slot mang lại nhiều tiền nhất:
```csharp
// SlotController.GetAvailableSlot()
// Duyệt toàn bộ slot Ready, chọn slot có Profit lớn nhất
```

**ScriptableObject read-only** — toàn bộ field là `[SerializeField] private` với getter only, tránh bị ghi đè khi build APK.

**Event-driven dispatch** — không dùng polling trong Update để tìm worker/customer rảnh, thay vào đó dùng event `OnSlotReady`, `OnCustomerWaiting`, `OnWorkerIdle` để trigger đúng lúc cần.

---

## Hướng phát triển tiếp theo

- Thêm hệ thống **Save/Load** dùng `PlayerPrefs` + JSON + SQL
- Hệ thống **Buff stack** từ nhiều nguồn (level + x2 slot + x2 global tính độc lập)
- Thêm hiệu ứng particle khi thu hoạch và giao hàng
- Offline earnings: tính toán profit tích lũy khi thoát game
