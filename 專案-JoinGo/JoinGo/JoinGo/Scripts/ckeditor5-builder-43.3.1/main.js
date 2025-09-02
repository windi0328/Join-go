import {
	AutoLink,
	ClassicEditor,
	AccessibilityHelp,
	Alignment,
	Autoformat,
	AutoImage,
	Autosave,
	BlockQuote,
	Bold,
	Essentials,
	FontBackgroundColor,
	FontColor,
	FontFamily,
	FontSize,
	GeneralHtmlSupport,
	Heading,
	Highlight,
	ImageBlock,
	ImageCaption,
	ImageInline,
	ImageInsert,
	ImageInsertViaUrl,
	ImageResize,
	ImageStyle,
	ImageTextAlternative,
	ImageToolbar,
	ImageUpload,
	Indent,
	IndentBlock,
	Italic,
	Link,
	LinkImage,
	List,
	ListProperties,
	MediaEmbed,
	Paragraph,
	PasteFromOffice,
	SelectAll,
	ShowBlocks,
	SimpleUploadAdapter,
	SourceEditing,
	SpecialCharacters,
	SpecialCharactersArrows,
	SpecialCharactersCurrency,
	SpecialCharactersEssentials,
	SpecialCharactersLatin,
	SpecialCharactersMathematical,
	SpecialCharactersText,
	Table,
	TableCaption,
	TableCellProperties,
	TableColumnResize,
	TableProperties,
	TableToolbar,
	TextTransformation,
	TodoList,
	Underline,
	Undo
} from '../ckeditor5-builder-43.3.1/ckeditor5/ckeditor5.js';

import translations from '../ckeditor5-builder-43.3.1/ckeditor5/translations/zh.js';




const editorConfig = {
	extraPlugins: [MyCustomUploadAdapterPlugin],
	toolbar: {
		items: [
			'undo',
			'redo',
			'|',
			'sourceEditing',
			'showBlocks',
			'|',
			'heading',
			'|',
			'fontSize',
			'fontFamily',
			'fontColor',
			'fontBackgroundColor',
			'|',
			'bold',
			'italic',
			'underline',
			'|',
			'specialCharacters',
			'link',
			'insertImage',
			'mediaEmbed',
			'insertTable',
			'highlight',
			'blockQuote',
			'|',
			'alignment',
			'|',
			'bulletedList',
			'numberedList',
			'todoList',
			'outdent',
			'indent'
		],
		shouldNotGroupWhenFull: true
	},
	plugins: [
		AutoLink,
		AccessibilityHelp,
		Alignment,
		Autoformat,
		AutoImage,
		Autosave,
		BlockQuote,
		Bold,
		Essentials,
		FontBackgroundColor,
		FontColor,
		FontFamily,
		FontSize,
		GeneralHtmlSupport,
		Heading,
		Highlight,
		ImageBlock,
		ImageCaption,
		ImageInline,
		ImageInsert,
		ImageInsertViaUrl,
		ImageResize,
		ImageStyle,
		ImageTextAlternative,
		ImageToolbar,
		ImageUpload,
		Indent,
		IndentBlock,
		Italic,
		Link,
		LinkImage,
		List,
		ListProperties,
		MediaEmbed,
		Paragraph,
		PasteFromOffice,
		SelectAll,
		ShowBlocks,
		SimpleUploadAdapter,
		SourceEditing,
		SpecialCharacters,
		SpecialCharactersArrows,
		SpecialCharactersCurrency,
		SpecialCharactersEssentials,
		SpecialCharactersLatin,
		SpecialCharactersMathematical,
		SpecialCharactersText,
		Table,
		TableCaption,
		TableCellProperties,
		TableColumnResize,
		TableProperties,
		TableToolbar,
		TextTransformation,
		TodoList,
		Underline,
		Undo
	],
	fontFamily: {
		supportAllValues: true
	},
	fontSize: {
		options: [10, 12, 14, 'default', 18, 20, 22],
		supportAllValues: true
	},
	heading: {
		options: [
			{
				model: 'paragraph',
				title: 'Paragraph',
				class: 'ck-heading_paragraph'
			},
			{
				model: 'heading1',
				view: 'h1',
				title: 'Heading 1',
				class: 'ck-heading_heading1'
			},
			{
				model: 'heading2',
				view: 'h2',
				title: 'Heading 2',
				class: 'ck-heading_heading2'
			},
			{
				model: 'heading3',
				view: 'h3',
				title: 'Heading 3',
				class: 'ck-heading_heading3'
			},
			{
				model: 'heading4',
				view: 'h4',
				title: 'Heading 4',
				class: 'ck-heading_heading4'
			},
			{
				model: 'heading5',
				view: 'h5',
				title: 'Heading 5',
				class: 'ck-heading_heading5'
			},
			{
				model: 'heading6',
				view: 'h6',
				title: 'Heading 6',
				class: 'ck-heading_heading6'
			}
		]
	},
	htmlSupport: {
		allow: [
			{
				name: /^.*$/,
				styles: true,
				attributes: true,
				classes: true
			}
		]
	},
	image: {
		toolbar: [
			'toggleImageCaption',
			'imageTextAlternative',
			'|',
			'imageStyle:inline',
			'imageStyle:wrapText',
			'imageStyle:breakText',
			'|',
			'resizeImage'
		]
	},
	initialData:
		'',
	language: 'zh',
	link: {
		addTargetToExternalLinks: true,
		defaultProtocol: 'https://',
		decorators: {
			toggleDownloadable: {
				mode: 'manual',
				label: 'Downloadable',
				attributes: {
					download: 'file'
				}
			}
		}
	},
	list: {
		properties: {
			styles: true,
			startIndex: true,
			reversed: true
		}
	},
	placeholder: '',
	table: {
		contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
	},
	translations: [translations]
};


// 儲存多個 CKEditor 實體
window.ckeditorInstances = {};

// 初始化 CKEditor
window.initializeCKEditor = function (editorId, data = '') {
	const selector = `.${editorId}`;
	const element = document.querySelector(selector);

	if (!element) {
		console.warn(`找不到 ${selector} 元素`);
		return;
	}

	ClassicEditor.create(element, editorConfig)
		.then(newEditor => {
			// 儲存實體到物件中
			window.ckeditorInstances[editorId] = newEditor;

			// 如果有初始內容就設定
			if (data) {
				data = data.replace(/font-size\s*:\s*(\d+(?:\.\d+)?)(px|pt)/gi, (match, num, unit) => {
					let pxValue = parseFloat(num);
					if (unit.toLowerCase() === 'pt') {
						pxValue *= 1.333; // pt -> px
					}
					let emValue = (pxValue / 16).toFixed(3).replace(/\.?0+$/, '') + 'em';
					return 'font-size: ' + emValue;
				});


				// 補 alt 屬性
				data = data.replace(/<img\b[^>]*>/gi, match => {
					// 找 alt 屬性值（如果有）
					const altMatch = match.match(/alt\s*=\s*(['"])(.*?)\1/i);
					if (altMatch) {
						// 如果 alt 值非空，回傳原本的標籤
						if (altMatch[2].trim() !== '') {
							return match;
						}
						// alt 是空字串，替換成有意義的文字
						return match.replace(/alt\s*=\s*(['"])(.*?)\1/i, 'alt="上傳圖片"');
					}
					// 沒有 alt 屬性，直接加上
					return match.replace(/<img/i, '<img alt="上傳圖片"');
				});



				// 補 <a> 沒有 title 的屬性
				data = data.replace(/<a\b[^>]*>/gi, match => {
					// 找 title 屬性（如果有）
					const titleMatch = match.match(/title\s*=\s*(['"])(.*?)\1/i);
					if (titleMatch) {
						// title 有值且不空，保留原本
						if (titleMatch[2].trim() !== '') {
							return match;
						}
						// title 是空字串，替換成預設文字
						return match.replace(/title\s*=\s*(['"])(.*?)\1/i, 'title="圖片連結"');
					}
					// 沒有 title 屬性，新增 title
					return match.replace(/<a/i, '<a title="圖片連結"');
				});


				// 補 <a> 沒有文字內容的情況 (注意：簡單替換會影響嵌套，僅對空白或無字元內容補充)
				data = data.replace(/<a([^>]*)>\s*<\/a>/gi, '<a$1>點擊查看詳情</a>');


				// 補 iframe 沒有 title 的屬性
				data = data.replace(/<iframe\b[^>]*>/gi, match => {
					const titleMatch = match.match(/title\s*=\s*(['"])(.*?)\1/i);
					if (titleMatch) {
						if (titleMatch[2].trim() !== '') {
							return match;
						}
						return match.replace(/title\s*=\s*(['"])(.*?)\1/i, 'title="嵌入影音內容區塊"');
					}
					return match.replace(/<iframe/i, '<iframe title="嵌入影音內容區塊"');
				});



				newEditor.setData(data);
			}


			setTimeout(() => {
				const fileInputs = document.querySelectorAll('input.ck-hidden[type="file"]');
				fileInputs.forEach(input => {
					// 只在尚未設置 aria-label 時補上
					if (!input.hasAttribute('aria-label')) {
						input.setAttribute('aria-label', '選擇要上傳的圖片');
					}
					if (!input.hasAttribute('title')) {
						input.setAttribute('title', '選擇要上傳的圖片');
					}
				});
			}, 500);

		
			const updateAccessibilityAttributes = () => {
				const model = newEditor.model;
				model.change(writer => {
					const root = model.document.getRoot();

					for (const item of root.getChildren()) {
						// 圖片補 alt
						if ((item.name === 'imageBlock' || item.name === 'imageInline') &&
							(!item.getAttribute('alt') || item.getAttribute('alt').trim() === '')) {
							writer.setAttribute('alt', '上傳圖片', item);
						}

						// 連結補文字與 title
						if (item.hasAttribute('linkHref')) {
							const children = Array.from(item.getChildren());
							const hasText = children.some(child => child.is('text') && child.data.trim().length > 0);

							if (!hasText) {
								writer.insertText('點擊查看詳情', item, 0);
							}

							const title = item.getAttribute('title');
							if (!title || title.trim() === '') {
								writer.setAttribute('title', '圖片連結', item);
							}

							// 確保 <a> 內若有圖片，圖片也有 alt
							for (const child of children) {
								if ((child.name === 'imageBlock' || child.name === 'imageInline') &&
									(!child.getAttribute('alt') || child.getAttribute('alt').trim() === '')) {
									writer.setAttribute('alt', '上傳圖片', child);
								}
							}
						}
					}
				});
			};

			newEditor.model.document.on('change:data', () => {
				updateAccessibilityAttributes();
			});

			newEditor.model.document.on('change', (evt, batch) => {
				if (!batch || batch.type !== 'transparent') {
					updateAccessibilityAttributes();
				}
			});





			// 自動補圖片 alt
			newEditor.model.document.on('change:data', () => {
				const model = newEditor.model;
				model.change(writer => {
					for (const item of model.document.getRoot().getChildren()) {
						if ((item.name === 'imageBlock' || item.name === 'imageInline') &&
							(!item.getAttribute('alt') || item.getAttribute('alt').trim() === '')
						) {
							writer.setAttribute('alt', '上傳圖片', item);
						}
					}
				});
			});

			newEditor.model.document.on('change', (evt, batch) => {
				if (batch && batch.type === 'transparent') return;

				const model = newEditor.model;

				model.change(writer => {
					for (const item of model.document.getRoot().getChildren()) {
						if (
							(item.name === 'imageBlock' || item.name === 'imageInline') &&
							(!item.getAttribute('alt') || item.getAttribute('alt').trim() === '')
						) {
							writer.setAttribute('alt', '上傳圖片', item);
						}
					}
				});
			});
		})
		.catch(error => {
			console.error(`CKEditor (${editorId}) 初始化錯誤:`, error);
		});
};





	// 銷毀 CKEditor
	window.destroyCKEditor = function () {
		if (window.ckeditor) {
			window.ckeditor.destroy()
				.then(() => {
					window.ckeditor = null;
				});
		}
	};

	// 提交表單時獲取 CKEditor 數據
	$('#addForm,#submitBtn').on('click', function () {
		if (window.ckeditor) {
			const editorData = window.ckeditor.getData();
			$('#CKContent').val(editorData);
		}
	});



//新增圖片
class MyUploadAdapter {
	constructor(loader) {
		this.loader = loader;
	}

	upload() {
		return new Promise((resolve, reject) => {
			this.loader.file.then(file => {
				const reader = new FileReader();
				const fileName = file.name;
				const fileType = file.type;

				// 當檔案讀取完成後的回調
				reader.onload = () => {
					const binaryData = reader.result; // 這裡獲取到的就是檔案的二進位資料
					const blob = new Blob([binaryData], { type: fileType });

					let formData = new FormData();
					formData.append("file", blob, fileName);

					// 將二進位資料發送到伺服器
					fetch('/Manage/CKUploadFile', {
						method: 'POST',
						body: formData, // 將二進位資料作為請求主體
					})
						.then(response => response.json())
						.then(result => {
							resolve({ default: result.url }); // 假設伺服器返回的 JSON 包含圖片的 URL
						})
						.catch(reject);
				};

				// 開始讀取檔案
				reader.readAsArrayBuffer(file); // 使用 readAsArrayBuffer 讀取檔案的二進位資料
			});
		});
	}

	abort() {
		// 可以在這裡處理上傳中止的邏輯
	}
}

function MyCustomUploadAdapterPlugin(editor) {
	editor.plugins.get('FileRepository').createUploadAdapter = loader => {
		return new MyUploadAdapter(loader);
	};
}