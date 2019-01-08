/******/ (function(modules) { // webpackBootstrap
/******/ 	// install a JSONP callback for chunk loading
/******/ 	function webpackJsonpCallback(data) {
/******/ 		var chunkIds = data[0];
/******/ 		var moreModules = data[1];
/******/ 		var executeModules = data[2];
/******/
/******/ 		// add "moreModules" to the modules object,
/******/ 		// then flag all "chunkIds" as loaded and fire callback
/******/ 		var moduleId, chunkId, i = 0, resolves = [];
/******/ 		for(;i < chunkIds.length; i++) {
/******/ 			chunkId = chunkIds[i];
/******/ 			if(installedChunks[chunkId]) {
/******/ 				resolves.push(installedChunks[chunkId][0]);
/******/ 			}
/******/ 			installedChunks[chunkId] = 0;
/******/ 		}
/******/ 		for(moduleId in moreModules) {
/******/ 			if(Object.prototype.hasOwnProperty.call(moreModules, moduleId)) {
/******/ 				modules[moduleId] = moreModules[moduleId];
/******/ 			}
/******/ 		}
/******/ 		if(parentJsonpFunction) parentJsonpFunction(data);
/******/
/******/ 		while(resolves.length) {
/******/ 			resolves.shift()();
/******/ 		}
/******/
/******/ 		// add entry modules from loaded chunk to deferred list
/******/ 		deferredModules.push.apply(deferredModules, executeModules || []);
/******/
/******/ 		// run deferred modules when all chunks ready
/******/ 		return checkDeferredModules();
/******/ 	};
/******/ 	function checkDeferredModules() {
/******/ 		var result;
/******/ 		for(var i = 0; i < deferredModules.length; i++) {
/******/ 			var deferredModule = deferredModules[i];
/******/ 			var fulfilled = true;
/******/ 			for(var j = 1; j < deferredModule.length; j++) {
/******/ 				var depId = deferredModule[j];
/******/ 				if(installedChunks[depId] !== 0) fulfilled = false;
/******/ 			}
/******/ 			if(fulfilled) {
/******/ 				deferredModules.splice(i--, 1);
/******/ 				result = __webpack_require__(__webpack_require__.s = deferredModule[0]);
/******/ 			}
/******/ 		}
/******/ 		return result;
/******/ 	}
/******/
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// object to store loaded and loading chunks
/******/ 	// undefined = chunk not loaded, null = chunk preloaded/prefetched
/******/ 	// Promise = chunk loading, 0 = chunk loaded
/******/ 	var installedChunks = {
/******/ 		"wwwroot/js/build/dev/pages/newnotification": 0
/******/ 	};
/******/
/******/ 	var deferredModules = [];
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	var jsonpArray = window["webpackJsonp"] = window["webpackJsonp"] || [];
/******/ 	var oldJsonpFunction = jsonpArray.push.bind(jsonpArray);
/******/ 	jsonpArray.push = webpackJsonpCallback;
/******/ 	jsonpArray = jsonpArray.slice();
/******/ 	for(var i = 0; i < jsonpArray.length; i++) webpackJsonpCallback(jsonpArray[i]);
/******/ 	var parentJsonpFunction = oldJsonpFunction;
/******/
/******/
/******/ 	// add entry module to deferred list
/******/ 	deferredModules.push(["./sources/pages/newnotification/index.js","common"]);
/******/ 	// run deferred modules when ready
/******/ 	return checkDeferredModules();
/******/ })
/************************************************************************/
/******/ ({

/***/ "./sources/fonts/style.css":
/*!*********************************!*\
  !*** ./sources/fonts/style.css ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/common/ConfirmPopup.js":
/*!**********************************************!*\
  !*** ./sources/pages/common/ConfirmPopup.js ***!
  \**********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _reactstrap = __webpack_require__(/*! reactstrap */ "./node_modules/reactstrap/dist/reactstrap.es.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var ConfirmPopup = function (_Component) {
  _inherits(ConfirmPopup, _Component);

  function ConfirmPopup(props) {
    _classCallCheck(this, ConfirmPopup);

    return _possibleConstructorReturn(this, (ConfirmPopup.__proto__ || Object.getPrototypeOf(ConfirmPopup)).call(this, props));
  }

  _createClass(ConfirmPopup, [{
    key: 'render',
    value: function render() {
      var _this2 = this;

      var _props = this.props,
          isShowPopup = _props.isShowPopup,
          title = _props.title,
          content = _props.content;

      var warningIcon = this.props.warningIcon ? _react2.default.createElement('div', { className: 'icon-warning' }) : null;

      return _react2.default.createElement(
        'div',
        null,
        _react2.default.createElement(
          _reactstrap.Modal,
          { isOpen: isShowPopup, toggle: this.props.confirmToggle, wrapClassName: 'popup-confirm' },
          _react2.default.createElement(
            _reactstrap.ModalHeader,
            { toggle: this.props.confirmToggle },
            title
          ),
          _react2.default.createElement(
            _reactstrap.ModalBody,
            null,
            warningIcon,
            _react2.default.createElement('div', { className: 'info', contentEditable: 'true', dangerouslySetInnerHTML: { __html: this.props.content } })
          ),
          _react2.default.createElement(
            _reactstrap.ModalFooter,
            null,
            _react2.default.createElement(
              'button',
              { className: 'btn-submit', onClick: function onClick() {
                  return _this2.props.confirmHandler(true);
                } },
              'OK'
            ),
            _react2.default.createElement(
              'button',
              { className: 'btn-cancel', onClick: function onClick() {
                  return _this2.props.confirmHandler(false);
                } },
              'Cancel'
            )
          )
        )
      );
    }
  }]);

  return ConfirmPopup;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(ConfirmPopup);

/***/ }),

/***/ "./sources/pages/common/breadcrumbs.js":
/*!*********************************************!*\
  !*** ./sources/pages/common/breadcrumbs.js ***!
  \*********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _reactstrap = __webpack_require__(/*! reactstrap */ "./node_modules/reactstrap/dist/reactstrap.es.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var NCBBreadcrum = function (_Component) {
  _inherits(NCBBreadcrum, _Component);

  function NCBBreadcrum(props) {
    _classCallCheck(this, NCBBreadcrum);

    return _possibleConstructorReturn(this, (NCBBreadcrum.__proto__ || Object.getPrototypeOf(NCBBreadcrum)).call(this, props));
  }

  _createClass(NCBBreadcrum, [{
    key: 'render',
    value: function render() {
      var _props = this.props,
          chatbotName = _props.chatbotName,
          pageTitle = _props.pageTitle;


      return _react2.default.createElement(
        _reactstrap.Breadcrumb,
        { className: 'nav-customized' },
        _react2.default.createElement(
          _reactstrap.BreadcrumbItem,
          null,
          chatbotName
        ),
        _react2.default.createElement(
          _reactstrap.BreadcrumbItem,
          { active: true },
          pageTitle
        )
      );
    }
  }]);

  return NCBBreadcrum;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(NCBBreadcrum);

/***/ }),

/***/ "./sources/pages/newnotification/App.js":
/*!**********************************************!*\
  !*** ./sources/pages/newnotification/App.js ***!
  \**********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _NewNotification = __webpack_require__(/*! ./components/NewNotification */ "./sources/pages/newnotification/components/NewNotification.js");

var _NewNotification2 = _interopRequireDefault(_NewNotification);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var App = function (_Component) {
  _inherits(App, _Component);

  function App(props) {
    _classCallCheck(this, App);

    var _this = _possibleConstructorReturn(this, (App.__proto__ || Object.getPrototypeOf(App)).call(this, props));

    _this.state = {
      isShow: false,
      titlePopup: '',
      contentPopup: '',
      editConfirmPopup: {
        title: "Edit Title & Content",
        content: "`You can <strong>edit Title & Content</strong> but manually change the text in {} might cause the parameter replacing feature dysfunctional`"
      },
      resetConfirmPopup: {
        title: "Reset Title & Content",
        content: "Reset <strong>Title & Content</strong> will recover the original template and discard your change"
      }
    };

    _this.toggle = _this.toggle.bind(_this);
    _this.contentEditConfirmPopup = _this.contentEditConfirmPopup.bind(_this);
    _this.contentResetConfirmPopup = _this.contentResetConfirmPopup.bind(_this);
    return _this;
  }

  _createClass(App, [{
    key: 'toggle',
    value: function toggle() {
      this.setState({
        isShow: !this.state.isShow
      });
    }
  }, {
    key: 'contentEditConfirmPopup',
    value: function contentEditConfirmPopup() {
      this.toggle();

      this.setState({
        titlePopup: this.state.editConfirmPopup.title,
        contentPopup: this.state.editConfirmPopup.content
      });
    }
  }, {
    key: 'contentResetConfirmPopup',
    value: function contentResetConfirmPopup() {
      this.toggle();

      this.setState({
        titlePopup: this.state.resetConfirmPopup.title,
        contentPopup: this.state.resetConfirmPopup.content
      });
    }
  }, {
    key: 'render',
    value: function render() {
      return _react2.default.createElement(_NewNotification2.default, {
        isShowPopup: this.state.isShow,
        titlePopup: this.state.titlePopup,
        contentPopup: this.state.contentPopup,
        toggle: this.toggle,
        contentEditConfirmPopupHandler: this.contentEditConfirmPopup,
        contentResetConfirmPopupHandler: this.contentResetConfirmPopup
      });
    }
  }]);

  return App;
}(_react.Component);

exports.default = (0, _reactRedux.connect)(function (state) {
  return state.login;
})(App);

/***/ }),

/***/ "./sources/pages/newnotification/components/NewNotification.js":
/*!*********************************************************************!*\
  !*** ./sources/pages/newnotification/components/NewNotification.js ***!
  \*********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _availityReactstrapValidation = __webpack_require__(/*! availity-reactstrap-validation */ "./node_modules/availity-reactstrap-validation/lib/index.js");

var _reactstrap = __webpack_require__(/*! reactstrap */ "./node_modules/reactstrap/dist/reactstrap.es.js");

var _ConfirmPopup = __webpack_require__(/*! ../../common/ConfirmPopup */ "./sources/pages/common/ConfirmPopup.js");

var _ConfirmPopup2 = _interopRequireDefault(_ConfirmPopup);

var _breadcrumbs = __webpack_require__(/*! ../../common/breadcrumbs */ "./sources/pages/common/breadcrumbs.js");

var _breadcrumbs2 = _interopRequireDefault(_breadcrumbs);

__webpack_require__(/*! ../../../fonts/style.css */ "./sources/fonts/style.css");

__webpack_require__(/*! ./NewNotification.scss */ "./sources/pages/newnotification/components/NewNotification.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var NewNotification = function (_Component) {
  _inherits(NewNotification, _Component);

  function NewNotification(props) {
    _classCallCheck(this, NewNotification);

    var _this = _possibleConstructorReturn(this, (NewNotification.__proto__ || Object.getPrototypeOf(NewNotification)).call(this, props));

    _this.toggle = _this.toggle.bind(_this);
    _this.contentEditConfirmPopupHandler = _this.contentEditConfirmPopupHandler.bind(_this);
    _this.contentResetConfirmPopupHandler = _this.contentResetConfirmPopupHandler.bind(_this);
    return _this;
  }

  _createClass(NewNotification, [{
    key: 'contentEditConfirmPopupHandler',
    value: function contentEditConfirmPopupHandler() {
      this.props.contentEditConfirmPopupHandler();
    }
  }, {
    key: 'contentResetConfirmPopupHandler',
    value: function contentResetConfirmPopupHandler() {
      this.props.contentResetConfirmPopupHandler();
    }
  }, {
    key: 'toggle',
    value: function toggle() {
      this.props.toggle();
    }
  }, {
    key: 'render',
    value: function render() {
      var templateContent = " Thank you for your kind attention!";
      var _props = this.props,
          isShowPopup = _props.isShowPopup,
          titlePopup = _props.titlePopup,
          contentPopup = _props.contentPopup;

      return _react2.default.createElement(
        'div',
        null,
        _react2.default.createElement(_breadcrumbs2.default, { chatbotName: 'Skype Chatbot', pageTitle: 'New Notification' }),
        _react2.default.createElement(
          'div',
          { className: 'content form' },
          _react2.default.createElement(
            _availityReactstrapValidation.AvForm,
            null,
            _react2.default.createElement(
              _availityReactstrapValidation.AvField,
              {
                type: 'select',
                name: 'notificationType',
                label: 'Notification Type',
                labelClass: 'form-label required',
                inputClass: 'notification-type',
                errorMessage: 'This field is required',
                required: true },
              _react2.default.createElement(
                'option',
                null,
                'UM Licensee 1'
              ),
              _react2.default.createElement(
                'option',
                null,
                'UM Licensee 2'
              )
            ),
            _react2.default.createElement(_availityReactstrapValidation.AvField, {
              name: 'recipient',
              label: 'Recipient',
              labelClass: 'form-label required',
              errorMessage: 'This field is required',
              required: true }),
            _react2.default.createElement(
              'div',
              { className: 'row-has-actions' },
              _react2.default.createElement(
                _availityReactstrapValidation.AvField,
                {
                  type: 'select',
                  name: 'notificationTemplate',
                  label: 'Notification Template',
                  labelClass: 'form-label required',
                  inputClass: 'notification-template',
                  errorMessage: 'This field is required',
                  required: true },
                _react2.default.createElement(
                  'option',
                  null,
                  'Weekly UM 1'
                ),
                _react2.default.createElement(
                  'option',
                  null,
                  'Weekly UM 2'
                )
              ),
              _react2.default.createElement(
                'div',
                { className: 'action', onClick: this.contentEditConfirmPopupHandler },
                _react2.default.createElement(
                  'div',
                  { className: 'circle orange' },
                  _react2.default.createElement('span', { className: 'icon-edit' })
                ),
                _react2.default.createElement(
                  'span',
                  { className: 'text' },
                  'Edit Title & Content'
                )
              ),
              _react2.default.createElement(
                'div',
                { className: 'action', onClick: this.contentResetConfirmPopupHandler },
                _react2.default.createElement(
                  'div',
                  { className: 'circle blue' },
                  _react2.default.createElement('span', { className: 'icon-reset' })
                ),
                _react2.default.createElement(
                  'span',
                  { className: 'text' },
                  'Reset Title & Content'
                )
              )
            ),
            _react2.default.createElement(_availityReactstrapValidation.AvField, {
              name: 'title',
              label: 'Title',
              labelClass: 'form-label required',
              errorMessage: 'This field is required',
              required: true }),
            _react2.default.createElement(_availityReactstrapValidation.AvField, {
              type: 'textarea',
              name: 'content',
              label: 'Content',
              labelClass: 'form-label content required',
              inputClass: 'content',
              value: templateContent,
              required: true }),
            _react2.default.createElement(
              'fieldset',
              { className: 'parameter-area' },
              _react2.default.createElement(
                'legend',
                { className: 'bold' },
                'Parameters'
              ),
              _react2.default.createElement(
                'div',
                { className: 'parameter-list' },
                _react2.default.createElement(_availityReactstrapValidation.AvField, {
                  name: 'UMDate',
                  label: '{UM date}',
                  labelClass: 'form-label',
                  inputClass: 'm-r-15px',
                  value: 'UM date' }),
                _react2.default.createElement(_availityReactstrapValidation.AvField, {
                  name: 'umWeekDateEN',
                  label: '{UM Week Date - EN}',
                  labelClass: 'form-label p-l-15px',
                  inputClass: 'm-r-15px',
                  value: 'UM Week Date - EN' }),
                _react2.default.createElement('div', { className: 'w-100' }),
                _react2.default.createElement(_availityReactstrapValidation.AvField, {
                  name: 'startTime',
                  label: '{Start time}',
                  labelClass: 'form-label',
                  inputClass: 'm-r-15px',
                  value: '11:00' }),
                _react2.default.createElement(_availityReactstrapValidation.AvField, {
                  name: 'endTime',
                  label: '{End time}',
                  labelClass: 'form-label',
                  value: 'End time' }),
                _react2.default.createElement('div', { className: 'w-100' }),
                _react2.default.createElement(_availityReactstrapValidation.AvField, {
                  name: 'umWeekDateCN',
                  label: '{UM Week Date - CN}',
                  labelClass: 'form-label',
                  value: 'hh:mm:ss AM/PM MM/DD/YYYY' })
              )
            ),
            _react2.default.createElement(
              'div',
              { className: 'send-date-area' },
              _react2.default.createElement(
                _availityReactstrapValidation.AvField,
                {
                  type: 'select',
                  name: 'senDateTime',
                  labelClass: 'form-label required',
                  label: 'Send Date/Time',
                  required: true },
                _react2.default.createElement(
                  'option',
                  null,
                  '---Please select---'
                ),
                _react2.default.createElement(
                  'option',
                  null,
                  'Send immediately'
                ),
                _react2.default.createElement(
                  'option',
                  null,
                  'Send at'
                )
              ),
              _react2.default.createElement(
                'div',
                null,
                _react2.default.createElement(
                  'div',
                  { className: 'calendar' },
                  _react2.default.createElement('input', { type: 'text', name: 'dateTime', id: 'dateTime', className: 'form-control date-time', disabled: true })
                ),
                _react2.default.createElement(
                  'span',
                  null,
                  '(GMT +8)'
                )
              )
            ),
            _react2.default.createElement(
              _reactstrap.Button,
              { className: 'btn-submit' },
              'Submit'
            )
          )
        ),
        _react2.default.createElement(_ConfirmPopup2.default, {
          showHideModalHandler: this.toggle,
          isShowPopup: isShowPopup,
          title: titlePopup,
          content: contentPopup,
          warningIcon: true
        })
      );
    }
  }]);

  return NewNotification;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(NewNotification);

/***/ }),

/***/ "./sources/pages/newnotification/components/NewNotification.scss":
/*!***********************************************************************!*\
  !*** ./sources/pages/newnotification/components/NewNotification.scss ***!
  \***********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/newnotification/index.js":
/*!************************************************!*\
  !*** ./sources/pages/newnotification/index.js ***!
  \************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


__webpack_require__(/*! bootstrap/dist/css/bootstrap.css */ "./node_modules/bootstrap/dist/css/bootstrap.css");

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactDom = __webpack_require__(/*! react-dom */ "./node_modules/react-dom/index.js");

var _reactDom2 = _interopRequireDefault(_reactDom);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _configureStore = __webpack_require__(/*! ./store/configureStore */ "./sources/pages/newnotification/store/configureStore.js");

var _configureStore2 = _interopRequireDefault(_configureStore);

var _App = __webpack_require__(/*! ./App */ "./sources/pages/newnotification/App.js");

var _App2 = _interopRequireDefault(_App);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

//import registerServiceWorker from './registerServiceWorker';

// Create browser history to use in the Redux store
var baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
//import 'bootstrap/dist/css/bootstrap-theme.css';
//import './index.css';

var history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
var initialState = window.initialReduxState;
var store = (0, _configureStore2.default)(history, initialState);

var rootElement = document.getElementById('root');

_reactDom2.default.render(_react2.default.createElement(
  _reactRedux.Provider,
  { store: store },
  _react2.default.createElement(_App2.default, null)
), rootElement);

//registerServiceWorker();

/***/ }),

/***/ "./sources/pages/newnotification/store/configureStore.js":
/*!***************************************************************!*\
  !*** ./sources/pages/newnotification/store/configureStore.js ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

exports.default = configureStore;

var _redux = __webpack_require__(/*! redux */ "./node_modules/redux/es/index.js");

var _reduxThunk = __webpack_require__(/*! redux-thunk */ "./node_modules/redux-thunk/es/index.js");

var _reduxThunk2 = _interopRequireDefault(_reduxThunk);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function configureStore(history, initialState) {
  var reducers = {};

  var middleware = [_reduxThunk2.default];

  // In development, use the browser's Redux dev tools extension if installed
  var enhancers = [];
  var isDevelopment = "development" === 'development';
  if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
    enhancers.push(window.devToolsExtension());
  }

  var rootReducer = (0, _redux.combineReducers)(_extends({}, reducers));

  return (0, _redux.createStore)(rootReducer, initialState, _redux.compose.apply(undefined, [_redux.applyMiddleware.apply(undefined, middleware)].concat(enhancers)));
}

/***/ })

/******/ });
//# sourceMappingURL=newnotification.chunk.js.map