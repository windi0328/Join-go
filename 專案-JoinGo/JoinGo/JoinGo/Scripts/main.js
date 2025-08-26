

$(document).ready(function () {

});
document.addEventListener("DOMContentLoaded", function () {

    var swiper = new Swiper(".mySwiper", {
        spaceBetween: 96,
        centeredSlides: true,
        speed: 800,
        cssMode: true,
        autoplay: {
            delay: 5000,
            disableOnInteraction: false
        },
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
        pagination: {
            el: ".swiper-pagination",
        },
        breakpoints: {
            992: {
                slidesPerView: 1.5, // 較大螢幕的設置
            },
            0: {
                slidesPerView: 1, // 手機版的設置，顯示一個內容
            },
        },
        mousewheel: true,
        keyboard: true,
    });

    var swiper = new Swiper(".resultsSwiper", {
        spaceBetween: 24,
        centeredSlides: true,
        speed: 800,
        grabCursor: true,
        cssMode: true,
        // allowTouchMove: true, 
        autoplay: {
            delay: 5000,
            disableOnInteraction: false
        },
        navigation: {
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
        },
        pagination: {
            el: ".swiper-pagination",
        },
        breakpoints: {
            1400: {
                slidesPerView: 4, // 較大螢幕的設置
            },
            992: {
                slidesPerView: 3.5, // 較大螢幕的設置
            },
            768: {
                slidesPerView: 2.2,
            },
            0: {
                slidesPerView: 1.2, // 手機版的設置，顯示一個內容
            },
        },
        mousewheel: true,
        keyboard: true,
    });
    var swiper = new Swiper(".chatbotSwiper", {
        spaceBetween: 24,
        centeredSlides: true,
        grabCursor: true,
        // cssMode: true, 
        allowTouchMove: true,
        navigation: {
            nextEl: ".swiper-button-next5",
            prevEl: ".swiper-button-prev5",
        },
        pagination: {
            el: ".swiper-pagination",
        },
        breakpoints: {
            992: {
                slidesPerView: 3, // 較大螢幕的設置
            },
            768: {
                slidesPerView: 2.5,
            },
            0: {
                slidesPerView: 1.5, // 手機版的設置，顯示一個內容
            },
        },
        mousewheel: true,
        keyboard: true,
    });

    $('.tabNav .btnSquare').on('click keydown', function (e) {
        // 確保僅處理 click 或 Enter/Space 鍵
        if (e.type === 'click' || (e.type === 'keydown' && (e.key === 'Enter' || e.key === ' '))) {
            e.preventDefault(); // 避免 Space 造成頁面滾動

            // 移除所有按鈕的 current 類別，並為當前按鈕加上 current
            $('.tabNav .btnSquare').removeClass('current');
            $(this).addClass('current');

            // 獲取當前按鈕的索引
            var index = $(this).index();

            // 先隱藏所有的 tabContent，再顯示對應索引的內容
            $('.tabContent .list').removeClass('current').hide();
            $('.tabContent .list').eq(index).addClass('current').show();
        }
    });

    // 預設顯示第一個 tabContent，隱藏其他內容
    $('.tabContent .list').hide();
    $('.tabContent .list.current').show();

    //搜尋結果,滾軸按鈕
    const scrollContainer = document.querySelector(".scrollContainer");
    const scrollRight = document.getElementById("scrollRight");

    if (scrollContainer && scrollRight) {
        scrollRight.addEventListener("click", function () {
            scrollContainer.scrollBy({
                left: 150,
                behavior: "smooth",
            });
        });
    }

    //智能小幫手聊天室窗
    // 自訂 Bootstrap Collapse 開啟時的動畫效果
    var chatbotToggle = document.getElementById("chatbot-toggle");

    chatbotToggle.addEventListener("show.bs.collapse", function () {
        chatbotToggle.classList.add("show");
    });

    chatbotToggle.addEventListener("hide.bs.collapse", function () {
        chatbotToggle.classList.remove("show");
    });

    // 訊息發送功能
    function sendMessage() {
        var input = document.getElementById("chatInput");
        var chatBody = document.getElementById("chatMessages");

        if (input.value.trim() !== "") {
            var userMessage = document.createElement("div");
            userMessage.classList.add("chat-message", "user-message");
            userMessage.textContent = input.value;

            chatBody.appendChild(userMessage);
            input.value = "";
            chatBody.scrollTop = chatBody.scrollHeight; // 滾動到底部
        }
    }

    // 綁定按鈕事件
    const sendBtn = document.getElementById("sendBtn");
    if (sendBtn) {
        sendBtn.addEventListener("click", sendMessage);
    }

    // 允許使用 Enter 鍵發送訊息
    const chatInput = document.getElementById("chatInput");
    if (chatInput) {
        chatInput.addEventListener("keypress", function (event) {
            if (event.key === "Enter") {
                sendMessage();
            }
        });
    }

    //推播關閉
    var pushBox = document.querySelector(".push-box");
    var closeButton = document.querySelector(".push-box .btn-close");

    if (closeButton && pushBox) {
        closeButton.addEventListener("click", function () {
            pushBox.style.opacity = "0";
            pushBox.style.visibility = "hidden";
            pushBox.style.transition = "opacity 0.3s ease-in-out, visibility 0.3s ease-in-out";
        });
    }

    //業務職掌滾動該畫面
    document.querySelectorAll(".btnSquare").forEach(btn => {
        btn.addEventListener("click", function () {
            document.querySelectorAll(".btnSquare").forEach(el => el.classList.remove("current")); // 先移除所有的 current
            this.classList.add("current"); // 加到被點擊的按鈕
        });

        //滾動置頂
        document.querySelector(".scroll-top-btn").addEventListener("click", function () {
            window.scrollTo({ top: 0, behavior: "smooth" });
        });

    });

    //找到勾選的 Checkbox 並批次下載
    const selectAllCheckbox = document.getElementById("selectAllCheckbox");
    const checkboxes = document.querySelectorAll(".download-checkbox");
    const downloadBtn = document.getElementById("downloadSelectedBtn");

    // 全選/取消全選
    selectAllCheckbox.addEventListener("change", function () {
        checkboxes.forEach(checkbox => {
            checkbox.checked = selectAllCheckbox.checked;
        });
    });

    // 下載已勾選檔案
    downloadBtn.addEventListener("click", function () {
        const selectedFiles = document.querySelectorAll(".download-checkbox:checked");

        if (selectedFiles.length === 0) {
            alert("請勾選要下載的檔案！");
            return;
        }

        selectedFiles.forEach(file => {
            const fileUrl = file.getAttribute("data-file");
            const link = document.createElement("a");
            link.href = fileUrl;
            link.setAttribute("download", "");
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
    });


    //常見問題
    const accordions = document.querySelectorAll(".accordion-button");

    accordions.forEach((button, index) => {
        button.setAttribute("tabindex", "0"); // 讓 Tab 鍵可選中

        button.addEventListener("keydown", function (event) {
            let nextAccordion = accordions[index + 1]; // 下一個 accordion-button
            let prevAccordion = accordions[index - 1]; // 上一個 accordion-button

            if (event.key === "ArrowRight" || event.key === "ArrowDown") {
                // **右方向鍵 (→) 或 下方向鍵 (↓)：移動到下一個問題**
                if (nextAccordion) {
                    nextAccordion.focus();
                }
            } else if (event.key === "ArrowLeft" || event.key === "ArrowUp") {
                // **左方向鍵 (←) 或 上方向鍵 (↑)：移動到上一個問題**
                if (prevAccordion) {
                    prevAccordion.focus();
                }
            } else if (event.key === "Enter" || event.key === " ") {
                // **Enter 或 Space：開啟或收合 accordion**
                event.preventDefault();
                button.click();
            }
        });
    });




});
//------截止線-----



function initFooterCollapse() {
    const btn = document.querySelector(".btncollapse-icon span");
    const collapseElement = document.getElementById("fatfoot");

    if (!btn || !collapseElement) {
        setTimeout(initFooterCollapse, 200); // 200ms 後再檢查一次
        return;
    }
    collapseElement.addEventListener("show.bs.collapse", function () {
        btn.textContent = "keyboard_arrow_up";
    });

    collapseElement.addEventListener("hide.bs.collapse", function () {
        btn.textContent = "keyboard_arrow_down";
    });
}

// 確保 `footer.html` 載入後執行 `initFooterCollapse`
initFooterCollapse();
