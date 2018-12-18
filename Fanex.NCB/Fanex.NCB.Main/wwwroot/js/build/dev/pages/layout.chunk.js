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
/******/ 		"wwwroot/js/build/dev/pages/layout": 0
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
/******/ 	deferredModules.push(["./sources/pages/layout/index.js","common"]);
/******/ 	// run deferred modules when ready
/******/ 	return checkDeferredModules();
/******/ })
/************************************************************************/
/******/ ({

/***/ "./sources/pages/layout/App.js":
/*!*************************************!*\
  !*** ./sources/pages/layout/App.js ***!
  \*************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRouter = __webpack_require__(/*! react-router */ "./node_modules/react-router/es/index.js");

var _Layout = __webpack_require__(/*! ./components/Layout */ "./sources/pages/layout/components/Layout.js");

var _Layout2 = _interopRequireDefault(_Layout);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }
//import Layout from './components/Layout';

//import FetchData from './components/FetchData';


//import Login from './components/Login';

var App = function (_Component) {
  _inherits(App, _Component);

  function App(props) {
    _classCallCheck(this, App);

    return _possibleConstructorReturn(this, (App.__proto__ || Object.getPrototypeOf(App)).call(this, props));
  }

  _createClass(App, [{
    key: 'render',
    value: function render() {
      return _react2.default.createElement(_Layout2.default, null);
    }
  }]);

  return App;
}(_react.Component);

exports.default = (0, _reactRedux.connect)(function (state) {
  return state.login;
})(App);

/***/ }),

/***/ "./sources/pages/layout/components/Content.js":
/*!****************************************************!*\
  !*** ./sources/pages/layout/components/Content.js ***!
  \****************************************************/
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

var _Menu = __webpack_require__(/*! ./Menu */ "./sources/pages/layout/components/Menu.js");

var _Menu2 = _interopRequireDefault(_Menu);

__webpack_require__(/*! ./Content.scss */ "./sources/pages/layout/components/Content.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Content = function (_Component) {
  _inherits(Content, _Component);

  function Content(props) {
    _classCallCheck(this, Content);

    var _this = _possibleConstructorReturn(this, (Content.__proto__ || Object.getPrototypeOf(Content)).call(this, props));

    _this.state = {
      currentUrl: "/NewNotification/Index"
    };

    _this.onMenuChange = _this.onMenuChange.bind(_this);
    return _this;
  }

  _createClass(Content, [{
    key: 'onMenuChange',
    value: function onMenuChange(url) {
      this.setState({
        currentUrl: url
      });
    }
  }, {
    key: 'render',
    value: function render() {
      return _react2.default.createElement(
        'div',
        { className: 'body-content' },
        _react2.default.createElement(
          'div',
          { className: 'left-panel' },
          _react2.default.createElement(_Menu2.default, { onChange: this.onMenuChange })
        ),
        _react2.default.createElement(
          'div',
          { className: 'content ' },
          _react2.default.createElement('iframe', { src: this.state.currentUrl })
        )
      );
    }
  }]);

  return Content;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(Content);

/***/ }),

/***/ "./sources/pages/layout/components/Content.scss":
/*!******************************************************!*\
  !*** ./sources/pages/layout/components/Content.scss ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/layout/components/Header.js":
/*!***************************************************!*\
  !*** ./sources/pages/layout/components/Header.js ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
    value: true
});

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

__webpack_require__(/*! ./Header.scss */ "./sources/pages/layout/components/Header.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

var Header = function Header(props) {
    return _react2.default.createElement(
        'div',
        { className: 'header' },
        _react2.default.createElement(
            'div',
            { className: 'col-sm-12 col-xs-12 top-header' },
            _react2.default.createElement(
                'ul',
                { className: 'list-unstyled pull-right b-ul__userinfo' },
                _react2.default.createElement(
                    'li',
                    { className: 'datetime' },
                    _react2.default.createElement('span', { className: 'icon-clock' }),
                    '2:26:04 PM Nov 30, 2018 GMT+8'
                ),
                _react2.default.createElement(
                    'li',
                    { className: 'user-info' },
                    _react2.default.createElement(
                        'a',
                        { 'data-toggle': 'dropdown' },
                        _react2.default.createElement(
                            'span',
                            { className: 'info-bar' },
                            'Welcome Sa',
                            _react2.default.createElement('span', { className: 'icon-arrow-user' })
                        )
                    ),
                    _react2.default.createElement(
                        'ul',
                        { className: 'dropdown-menu menu-user-info', role: 'menu' },
                        _react2.default.createElement(
                            'li',
                            { className: 'b-li__userinfo' },
                            _react2.default.createElement(
                                'a',
                                { className: 'userinfo-feature', href: '/Auth/Index' },
                                _react2.default.createElement('span', { className: 'icon-signout' }),
                                'Sign out'
                            )
                        )
                    )
                )
            )
        )
    );
};

exports.default = (0, _reactRedux.connect)()(Header);

/***/ }),

/***/ "./sources/pages/layout/components/Header.scss":
/*!*****************************************************!*\
  !*** ./sources/pages/layout/components/Header.scss ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/layout/components/Layout.js":
/*!***************************************************!*\
  !*** ./sources/pages/layout/components/Layout.js ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

__webpack_require__(/*! ./Layout.scss */ "./sources/pages/layout/components/Layout.scss");

var _Header = __webpack_require__(/*! ./Header */ "./sources/pages/layout/components/Header.js");

var _Header2 = _interopRequireDefault(_Header);

var _Content = __webpack_require__(/*! ./Content */ "./sources/pages/layout/components/Content.js");

var _Content2 = _interopRequireDefault(_Content);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

var Layout = function Layout(props) {
  return _react2.default.createElement(
    'div',
    null,
    _react2.default.createElement(_Header2.default, null),
    _react2.default.createElement(_Content2.default, null)
  );
};

exports.default = (0, _reactRedux.connect)()(Layout);

/***/ }),

/***/ "./sources/pages/layout/components/Layout.scss":
/*!*****************************************************!*\
  !*** ./sources/pages/layout/components/Layout.scss ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/layout/components/Menu.js":
/*!*************************************************!*\
  !*** ./sources/pages/layout/components/Menu.js ***!
  \*************************************************/
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

__webpack_require__(/*! ./Menu.scss */ "./sources/pages/layout/components/Menu.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Menu = function (_Component) {
    _inherits(Menu, _Component);

    function Menu(props) {
        _classCallCheck(this, Menu);

        var _this = _possibleConstructorReturn(this, (Menu.__proto__ || Object.getPrototypeOf(Menu)).call(this, props));

        _this.onChange = _this.onChange.bind(_this);
        return _this;
    }

    _createClass(Menu, [{
        key: 'onChange',
        value: function onChange(url) {
            this.props.onChange(url);
        }
    }, {
        key: 'render',
        value: function render() {
            var _this2 = this;

            return _react2.default.createElement(
                'div',
                { role: 'tabpanel', className: 'leftmenu-tab', 'data-ng-cloak': true },
                _react2.default.createElement(
                    'ul',
                    { className: 'nav nav-tabs', role: 'tablist' },
                    _react2.default.createElement(
                        'li',
                        { role: 'presentation', className: 'active' },
                        _react2.default.createElement(
                            'div',
                            { role: 'tab', 'data-toggle': 'tab', 'data-target': '#tab-menu' },
                            'Menu'
                        )
                    )
                ),
                _react2.default.createElement(
                    'div',
                    { className: 'tab-content' },
                    _react2.default.createElement(
                        'div',
                        { role: 'tabpanel', className: 'menu-tab-content tab-pane active', id: 'tab-menu' },
                        _react2.default.createElement(
                            'ul',
                            { className: 'list-unstyled' },
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item', 'data-ng-repeat': 'menu in vm.menus' },
                                _react2.default.createElement(
                                    'div',
                                    { className: 'menu-item-content', 'data-ng-click': 'vm.menuClick($index);' },
                                    _react2.default.createElement('span', { className: 'icon-client' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'menu-title', 'data-ng-bind': '::menu.Name' },
                                        'Skype Chatbot'
                                    ),
                                    _react2.default.createElement('span', { className: 'pull-right', 'data-ng-class': 'icon-chevron-down' })
                                ),
                                _react2.default.createElement(
                                    'ul',
                                    { 'class': 'list-unstyled b-ul__submenu', 'data-ng-hide': '!menu.selected && vm.expandAsideMenu' },
                                    _react2.default.createElement(
                                        'li',
                                        { className: 'submenu-item', 'data-ng-class': '{active: submenu.selected}',
                                            'data-ng-repeat': 'submenu in menu.SubMenus' },
                                        _react2.default.createElement(
                                            'div',
                                            { className: 'submenu-item-content', 'data-ng-click': 'vm.subMenuClick($parent.$index, $index)' },
                                            _react2.default.createElement(
                                                'div',
                                                { onClick: function onClick() {
                                                        return _this2.onChange("/NewNotification/Index");
                                                    } },
                                                'New Notification'
                                            )
                                        )
                                    ),
                                    _react2.default.createElement(
                                        'li',
                                        { className: 'submenu-item', 'data-ng-class': '{active: submenu.selected}',
                                            'data-ng-repeat': 'submenu in menu.SubMenus' },
                                        _react2.default.createElement(
                                            'div',
                                            { className: 'submenu-item-content', 'data-ng-click': 'vm.subMenuClick($parent.$index, $index)' },
                                            _react2.default.createElement(
                                                'div',
                                                { onClick: function onClick() {
                                                        return _this2.onChange("/NotificationList/Index");
                                                    } },
                                                'Notification List'
                                            )
                                        )
                                    ),
                                    _react2.default.createElement(
                                        'li',
                                        { className: 'submenu-item', 'data-ng-class': '{active: submenu.selected}',
                                            'data-ng-repeat': 'submenu in menu.SubMenus' },
                                        _react2.default.createElement(
                                            'div',
                                            { className: 'submenu-item-content', 'data-ng-click': 'vm.subMenuClick($parent.$index, $index)' },
                                            _react2.default.createElement(
                                                'div',
                                                { onClick: function onClick() {
                                                        return _this2.onChange("/NotificationTemplate/Index");
                                                    } },
                                                'Notification Template'
                                            )
                                        )
                                    ),
                                    _react2.default.createElement(
                                        'li',
                                        { className: 'submenu-item', 'data-ng-class': '{active: submenu.selected}',
                                            'data-ng-repeat': 'submenu in menu.SubMenus' },
                                        _react2.default.createElement(
                                            'div',
                                            { className: 'submenu-item-content', 'data-ng-click': 'vm.subMenuClick($parent.$index, $index)' },
                                            _react2.default.createElement(
                                                'div',
                                                { onClick: function onClick() {
                                                        return _this2.onChange("/ChatGroupMapping/Index");
                                                    } },
                                                'Chat Group Mapping'
                                            )
                                        )
                                    ),
                                    _react2.default.createElement(
                                        'li',
                                        { className: 'submenu-item', 'data-ng-class': '{active: submenu.selected}',
                                            'data-ng-repeat': 'submenu in menu.SubMenus' },
                                        _react2.default.createElement(
                                            'div',
                                            { className: 'submenu-item-content', 'data-ng-click': 'vm.subMenuClick($parent.$index, $index)' },
                                            _react2.default.createElement(
                                                'div',
                                                { onClick: function onClick() {
                                                        return _this2.onChange("/NotificationType/Index");
                                                    } },
                                                'Notification Type'
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }
    }]);

    return Menu;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(Menu);

/***/ }),

/***/ "./sources/pages/layout/components/Menu.scss":
/*!***************************************************!*\
  !*** ./sources/pages/layout/components/Menu.scss ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/layout/index.js":
/*!***************************************!*\
  !*** ./sources/pages/layout/index.js ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


__webpack_require__(/*! bootstrap/dist/css/bootstrap.css */ "./node_modules/bootstrap/dist/css/bootstrap.css");

__webpack_require__(/*! bootstrap/dist/css/bootstrap-theme.css */ "./node_modules/bootstrap/dist/css/bootstrap-theme.css");

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactDom = __webpack_require__(/*! react-dom */ "./node_modules/react-dom/index.js");

var _reactDom2 = _interopRequireDefault(_reactDom);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _reactRouterRedux = __webpack_require__(/*! react-router-redux */ "./node_modules/react-router-redux/es/index.js");

var _history = __webpack_require__(/*! history */ "./node_modules/history/es/index.js");

var _configureStore = __webpack_require__(/*! ./store/configureStore */ "./sources/pages/layout/store/configureStore.js");

var _configureStore2 = _interopRequireDefault(_configureStore);

var _App = __webpack_require__(/*! ./App */ "./sources/pages/layout/App.js");

var _App2 = _interopRequireDefault(_App);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

//import registerServiceWorker from './registerServiceWorker';

// Create browser history to use in the Redux store
var baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
//import './index.css';

var history = (0, _history.createBrowserHistory)({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
var initialState = window.initialReduxState;
var store = (0, _configureStore2.default)(history, initialState);

var rootElement = document.getElementById('layout');

_reactDom2.default.render(_react2.default.createElement(
  _reactRedux.Provider,
  { store: store },
  _react2.default.createElement(
    _reactRouterRedux.ConnectedRouter,
    { history: history },
    _react2.default.createElement(_App2.default, null)
  )
), rootElement);

//registerServiceWorker();

/***/ }),

/***/ "./sources/pages/layout/store/configureStore.js":
/*!******************************************************!*\
  !*** ./sources/pages/layout/store/configureStore.js ***!
  \******************************************************/
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

var _reactRouterRedux = __webpack_require__(/*! react-router-redux */ "./node_modules/react-router-redux/es/index.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function configureStore(history, initialState) {
  var reducers = {};

  var middleware = [_reduxThunk2.default, (0, _reactRouterRedux.routerMiddleware)(history)];

  // In development, use the browser's Redux dev tools extension if installed
  var enhancers = [];
  var isDevelopment = "development" === 'development';
  if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
    enhancers.push(window.devToolsExtension());
  }

  var rootReducer = (0, _redux.combineReducers)(_extends({}, reducers, {
    routing: _reactRouterRedux.routerReducer
  }));

  return (0, _redux.createStore)(rootReducer, initialState, _redux.compose.apply(undefined, [_redux.applyMiddleware.apply(undefined, middleware)].concat(enhancers)));
}

/***/ })

/******/ });
//# sourceMappingURL=layout.chunk.js.map