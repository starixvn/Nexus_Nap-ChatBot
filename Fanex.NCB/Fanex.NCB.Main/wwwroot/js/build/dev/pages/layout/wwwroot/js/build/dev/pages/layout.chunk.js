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

/***/ "./sources/fonts/style.css":
/*!*********************************!*\
  !*** ./sources/fonts/style.css ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/images/Logo.svg":
/*!*********************************!*\
  !*** ./sources/images/Logo.svg ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "/images/483d9b10c23f7c000d28fd53bd772e43-Logo.svg";

/***/ }),

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

var _Header = __webpack_require__(/*! ./Header */ "./sources/pages/layout/components/Header.js");

var _Header2 = _interopRequireDefault(_Header);

__webpack_require__(/*! ./content.scss */ "./sources/pages/layout/components/content.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Content = function (_Component) {
  _inherits(Content, _Component);

  function Content(props) {
    _classCallCheck(this, Content);

    return _possibleConstructorReturn(this, (Content.__proto__ || Object.getPrototypeOf(Content)).call(this, props));
  }

  _createClass(Content, [{
    key: 'render',
    value: function render() {
      return _react2.default.createElement(
        'div',
        { className: 'main-container' },
        _react2.default.createElement(_Header2.default, {
          collapseExpandHandler: this.props.collapseExpandHandler,
          isSidebarExpanded: this.props.isSidebarExpanded }),
        _react2.default.createElement(
          'div',
          { className: 'content ' },
          _react2.default.createElement('iframe', { className: 'main-content', src: this.props.contentUrl })
        )
      );
    }
  }]);

  return Content;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(Content);

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

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

__webpack_require__(/*! ./Header.scss */ "./sources/pages/layout/components/Header.scss");

var _reactstrap = __webpack_require__(/*! reactstrap */ "./node_modules/reactstrap/dist/reactstrap.es.js");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Header = function (_Component) {
    _inherits(Header, _Component);

    function Header(props) {
        _classCallCheck(this, Header);

        var _this = _possibleConstructorReturn(this, (Header.__proto__ || Object.getPrototypeOf(Header)).call(this, props));

        _this.state = {
            dropdownOpen: false
        };

        _this.collapseExpandHandler = _this.collapseExpandHandler.bind(_this);
        _this.toggle = _this.toggle.bind(_this);
        return _this;
    }

    _createClass(Header, [{
        key: 'toggle',
        value: function toggle() {
            this.setState(function (prevState) {
                return {
                    dropdownOpen: !prevState.dropdownOpen
                };
            });
        }
    }, {
        key: 'collapseExpandHandler',
        value: function collapseExpandHandler() {
            this.props.collapseExpandHandler();
        }
    }, {
        key: 'render',
        value: function render() {
            var activeClass = this.props.isSidebarExpanded ? "line-wrapp" : 'line-wrapp collaped';
            return _react2.default.createElement(
                'div',
                { className: 'header' },
                _react2.default.createElement(
                    'div',
                    { className: activeClass, onClick: this.collapseExpandHandler },
                    _react2.default.createElement('div', { className: 'line top' }),
                    _react2.default.createElement('div', { className: 'line center' }),
                    _react2.default.createElement('div', { className: 'line bottom' })
                ),
                _react2.default.createElement(
                    'ul',
                    { className: 'list-unstyled userinfo' },
                    _react2.default.createElement(
                        'li',
                        { className: 'annoucement' },
                        _react2.default.createElement('div', { className: 'icon-bell' }),
                        _react2.default.createElement('div', { className: 'pulse-css' }),
                        _react2.default.createElement(
                            'div',
                            { className: 'number' },
                            '99+'
                        )
                    ),
                    _react2.default.createElement(
                        'li',
                        { className: 'user-info' },
                        _react2.default.createElement(
                            _reactstrap.Dropdown,
                            { isOpen: this.state.dropdownOpen, toggle: this.toggle },
                            _react2.default.createElement(
                                _reactstrap.DropdownToggle,
                                {
                                    tag: 'a' },
                                _react2.default.createElement(
                                    'div',
                                    { className: 'profile-picture' },
                                    _react2.default.createElement('span', { className: 'icon-username' })
                                ),
                                'Welcome Sa'
                            ),
                            _react2.default.createElement(
                                _reactstrap.DropdownMenu,
                                null,
                                _react2.default.createElement(
                                    _reactstrap.DropdownItem,
                                    { tag: 'a', href: '#' },
                                    _react2.default.createElement('span', { className: 'icon-change-pass' }),
                                    'Change Password'
                                ),
                                _react2.default.createElement(
                                    _reactstrap.DropdownItem,
                                    { tag: 'a', href: '/Auth/Index' },
                                    _react2.default.createElement('span', { className: 'icon-sign-out' }),
                                    'Sign Out'
                                )
                            )
                        )
                    )
                )
            );
        }
    }]);

    return Header;
}(_react.Component);

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

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

__webpack_require__(/*! ../../../fonts/style.css */ "./sources/fonts/style.css");

var _Sidebar = __webpack_require__(/*! ./Sidebar */ "./sources/pages/layout/components/Sidebar.js");

var _Sidebar2 = _interopRequireDefault(_Sidebar);

var _Content = __webpack_require__(/*! ./Content */ "./sources/pages/layout/components/Content.js");

var _Content2 = _interopRequireDefault(_Content);

__webpack_require__(/*! ./Layout.scss */ "./sources/pages/layout/components/Layout.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Layout = function (_Component) {
  _inherits(Layout, _Component);

  function Layout(props) {
    _classCallCheck(this, Layout);

    var _this = _possibleConstructorReturn(this, (Layout.__proto__ || Object.getPrototypeOf(Layout)).call(this, props));

    _this.state = {
      expandedClass: "sidebar-expanded",
      currentUrl: "/NewNotification/Index",
      isSidebarExpanded: true
    };

    _this.onMenuChange = _this.onMenuChange.bind(_this);
    _this.collapseExpandHandler = _this.collapseExpandHandler.bind(_this);
    return _this;
  }

  _createClass(Layout, [{
    key: 'onMenuChange',
    value: function onMenuChange(url) {
      this.setState({
        currentUrl: url
      });
    }
  }, {
    key: 'collapseExpandHandler',
    value: function collapseExpandHandler() {
      this.setState({
        isSidebarExpanded: !this.state.isSidebarExpanded
      });
    }
  }, {
    key: 'render',
    value: function render() {
      return _react2.default.createElement(
        'div',
        { className: 'page ' + (this.state.isSidebarExpanded ? "sidebar-expanded" : "sidebar-collapsed") },
        _react2.default.createElement(_Sidebar2.default, { onChange: this.onMenuChange }),
        _react2.default.createElement(_Content2.default, {
          contentUrl: this.state.currentUrl,
          isSidebarExpanded: this.state.isSidebarExpanded,
          collapseExpandHandler: this.collapseExpandHandler })
      );
    }
  }]);

  return Layout;
}(_react.Component);

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

/***/ "./sources/pages/layout/components/Sidebar.js":
/*!****************************************************!*\
  !*** ./sources/pages/layout/components/Sidebar.js ***!
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

var _Logo = __webpack_require__(/*! ../../../images/Logo.svg */ "./sources/images/Logo.svg");

var _Logo2 = _interopRequireDefault(_Logo);

__webpack_require__(/*! ./Sidebar.scss */ "./sources/pages/layout/components/Sidebar.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Sidebar = function (_Component) {
    _inherits(Sidebar, _Component);

    function Sidebar(props) {
        _classCallCheck(this, Sidebar);

        var _this = _possibleConstructorReturn(this, (Sidebar.__proto__ || Object.getPrototypeOf(Sidebar)).call(this, props));

        _this.onChange = _this.onChange.bind(_this);
        return _this;
    }

    _createClass(Sidebar, [{
        key: 'onChange',
        value: function onChange(e, url) {
            e.preventDefault();
            console.log(url);
            this.props.onChange(url);
        }
    }, {
        key: 'render',
        value: function render() {
            var _this2 = this;

            return _react2.default.createElement(
                'div',
                { className: 'sidebar' },
                _react2.default.createElement(
                    'div',
                    { className: 'nav-top' },
                    _react2.default.createElement('img', { src: _Logo2.default, className: 'logo' }),
                    _react2.default.createElement(
                        'h1',
                        { className: 'site-name' },
                        'Chatbot'
                    )
                ),
                _react2.default.createElement(
                    'ul',
                    { className: 'nav' },
                    _react2.default.createElement(
                        'li',
                        { className: 'menu' },
                        _react2.default.createElement(
                            'span',
                            { className: 'menu-text' },
                            'SKype Chatbot'
                        ),
                        _react2.default.createElement(
                            'ul',
                            { className: 'submenu list-unstyled' },
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item' },
                                _react2.default.createElement(
                                    'a',
                                    { href: '', className: 'link', onClick: function onClick(event) {
                                            return _this2.onChange(event, "/NewNotification/Index");
                                        } },
                                    _react2.default.createElement('span', { className: 'icon-new-notification' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'text' },
                                        'New Notification'
                                    )
                                )
                            ),
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item' },
                                _react2.default.createElement(
                                    'a',
                                    { href: '', className: 'link', onClick: function onClick(event) {
                                            return _this2.onChange(event, "/NotificationList/Index");
                                        } },
                                    _react2.default.createElement('span', { className: 'icon-notification-list' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'text' },
                                        'Notification List'
                                    )
                                )
                            ),
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item' },
                                _react2.default.createElement(
                                    'a',
                                    { href: '', className: 'link', onClick: function onClick(event) {
                                            return _this2.onChange(event, "/NotificationTemplate/Index");
                                        } },
                                    _react2.default.createElement('span', { className: 'icon-template' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'text' },
                                        'Notification Template'
                                    )
                                )
                            ),
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item' },
                                _react2.default.createElement(
                                    'a',
                                    { href: '', className: 'link', onClick: function onClick(event) {
                                            return _this2.onChange(event, "/ChatGroupMapping/Index");
                                        } },
                                    _react2.default.createElement('span', { className: 'icon-group-mapping' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'text' },
                                        'Chat Group Mapping'
                                    )
                                )
                            ),
                            _react2.default.createElement(
                                'li',
                                { className: 'menu-item' },
                                _react2.default.createElement(
                                    'a',
                                    { href: '', className: 'link', onClick: function onClick(event) {
                                            return _this2.onChange(event, "/NotificationType/Index");
                                        } },
                                    _react2.default.createElement('span', { className: 'icon-notification-type' }),
                                    _react2.default.createElement(
                                        'span',
                                        { className: 'text' },
                                        'Notification Type'
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }
    }]);

    return Sidebar;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(Sidebar);

/***/ }),

/***/ "./sources/pages/layout/components/Sidebar.scss":
/*!******************************************************!*\
  !*** ./sources/pages/layout/components/Sidebar.scss ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/layout/components/content.scss":
/*!******************************************************!*\
  !*** ./sources/pages/layout/components/content.scss ***!
  \******************************************************/
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

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactDom = __webpack_require__(/*! react-dom */ "./node_modules/react-dom/index.js");

var _reactDom2 = _interopRequireDefault(_reactDom);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _configureStore = __webpack_require__(/*! ./store/configureStore */ "./sources/pages/layout/store/configureStore.js");

var _configureStore2 = _interopRequireDefault(_configureStore);

var _App = __webpack_require__(/*! ./App */ "./sources/pages/layout/App.js");

var _App2 = _interopRequireDefault(_App);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

// Get the application-wide store instance, prepopulating with state from the server where available.
var initialState = window.initialReduxState;
var store = (0, _configureStore2.default)(history, initialState);

var rootElement = document.getElementById('layout');

_reactDom2.default.render(_react2.default.createElement(
  _reactRedux.Provider,
  { store: store },
  _react2.default.createElement(_App2.default, null)
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
//# sourceMappingURL=layout.chunk.js.map