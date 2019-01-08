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
/******/ 		"wwwroot/js/build/dev/pages/notificationlist": 0
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
/******/ 	deferredModules.push(["./sources/pages/notificationlist/index.js","common"]);
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

/***/ "./sources/pages/notificationlist/App.js":
/*!***********************************************!*\
  !*** ./sources/pages/notificationlist/App.js ***!
  \***********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _NotificationList = __webpack_require__(/*! ./components/NotificationList */ "./sources/pages/notificationlist/components/NotificationList.js");

var _NotificationList2 = _interopRequireDefault(_NotificationList);

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
      modal: false
    };

    _this.toggle = _this.toggle.bind(_this);
    return _this;
  }

  _createClass(App, [{
    key: 'toggle',
    value: function toggle() {
      this.setState({
        modal: !this.state.modal
      });
    }
  }, {
    key: 'render',
    value: function render() {
      return _react2.default.createElement(_NotificationList2.default, {
        isModal: this.state.modal,
        toggleHandler: this.toggle });
    }
  }]);

  return App;
}(_react.Component);

exports.default = (0, _reactRedux.connect)(function (state) {
  return state.login;
})(App);

/***/ }),

/***/ "./sources/pages/notificationlist/components/ErrorReport.js":
/*!******************************************************************!*\
  !*** ./sources/pages/notificationlist/components/ErrorReport.js ***!
  \******************************************************************/
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

var ErrorReport = function (_Component) {
  _inherits(ErrorReport, _Component);

  function ErrorReport(props) {
    _classCallCheck(this, ErrorReport);

    var _this = _possibleConstructorReturn(this, (ErrorReport.__proto__ || Object.getPrototypeOf(ErrorReport)).call(this, props));

    _this.toggleHandler = _this.toggleHandler.bind(_this);
    return _this;
  }

  _createClass(ErrorReport, [{
    key: 'toggleHandler',
    value: function toggleHandler() {
      this.props.toggleHandler();
    }
  }, {
    key: 'render',
    value: function render() {
      var _props = this.props,
          isModal = _props.isModal,
          isRemark = _props.isRemark;


      return _react2.default.createElement(
        'div',
        null,
        _react2.default.createElement(
          _reactstrap.Modal,
          { isOpen: isModal, toggle: this.toggleHandler, className: 'error-report' },
          _react2.default.createElement(
            _reactstrap.ModalHeader,
            { toggle: this.toggleHandler },
            'Notification Sending Error Report'
          ),
          _react2.default.createElement(
            _reactstrap.ModalBody,
            null,
            _react2.default.createElement(
              'p',
              null,
              'Error report for the notification titled:',
              _react2.default.createElement('br', null),
              _react2.default.createElement(
                'span',
                { className: 'bold text-uppercase' },
                'Weekly maintenance announcement - 12/26/2018'
              )
            ),
            _react2.default.createElement(
              'p',
              null,
              'Send at:',
              _react2.default.createElement('br', null),
              _react2.default.createElement(
                'span',
                { className: 'bold' },
                '12/26/2018 9:00:00 PM'
              )
            ),
            _react2.default.createElement(
              'div',
              { className: 'msg-success' },
              'Notification has been successfully sent to the following recipient(s): ',
              _react2.default.createElement(
                'strong',
                null,
                '12Bet, ABCbet'
              )
            ),
            _react2.default.createElement(
              'div',
              { className: 'msg-error' },
              'Error(s) occoured while sending the notification to the following recipient(s)'
            ),
            _react2.default.createElement(
              'table',
              { className: 'tbl w-100' },
              _react2.default.createElement(
                'thead',
                null,
                _react2.default.createElement(
                  'tr',
                  null,
                  _react2.default.createElement(
                    'th',
                    null,
                    _react2.default.createElement(
                      'div',
                      { className: 'has-icons' },
                      'Recipient List ',
                      _react2.default.createElement('span', { className: 'icon-copy' })
                    )
                  ),
                  _react2.default.createElement(
                    'th',
                    null,
                    'Recipient Type'
                  ),
                  _react2.default.createElement(
                    'th',
                    null,
                    'Reason'
                  )
                )
              ),
              _react2.default.createElement(
                'tbody',
                null,
                _react2.default.createElement(
                  'tr',
                  null,
                  _react2.default.createElement(
                    'td',
                    null,
                    'XYZbet'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Licensee'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Licensee is not mapped'
                  )
                ),
                _react2.default.createElement(
                  'tr',
                  null,
                  _react2.default.createElement(
                    'td',
                    null,
                    'BBIN'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    '3rd Party'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Chatbot has been removed from the group'
                  )
                ),
                _react2.default.createElement(
                  'tr',
                  null,
                  _react2.default.createElement(
                    'td',
                    null,
                    'BBIN2'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Licensee'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Chatbot has been removed from the group'
                  )
                ),
                _react2.default.createElement(
                  'tr',
                  null,
                  _react2.default.createElement(
                    'td',
                    null,
                    'Gold Deluxe'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Licensee'
                  ),
                  _react2.default.createElement(
                    'td',
                    null,
                    'Error occurred on Chatbot service'
                  )
                )
              )
            ),
            isRemark == "true" && _react2.default.createElement(
              'div',
              { className: 'remark' },
              _react2.default.createElement(
                'span',
                null,
                'Remark:'
              ),
              _react2.default.createElement('input', { type: 'text', className: 'form-control' }),
              _react2.default.createElement(
                'button',
                { className: 'btn-mark' },
                'Mark As Resolved'
              )
            ),
            isRemark == "false" && _react2.default.createElement(
              'div',
              { className: 'resolved-remark' },
              _react2.default.createElement(
                'p',
                null,
                'Remark: ',
                _react2.default.createElement(
                  'span',
                  { className: 'text-italic' },
                  'Manually sending the notification'
                )
              ),
              _react2.default.createElement(
                'p',
                null,
                _react2.default.createElement(
                  'span',
                  { className: 'text-red' },
                  '*'
                ),
                ' Resolved by: ',
                _react2.default.createElement(
                  'strong',
                  null,
                  'User01'
                )
              )
            )
          )
        )
      );
    }
  }]);

  return ErrorReport;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(ErrorReport);

/***/ }),

/***/ "./sources/pages/notificationlist/components/NotificationList.js":
/*!***********************************************************************!*\
  !*** ./sources/pages/notificationlist/components/NotificationList.js ***!
  \***********************************************************************/
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

var _breadcrumbs = __webpack_require__(/*! ../../common/breadcrumbs */ "./sources/pages/common/breadcrumbs.js");

var _breadcrumbs2 = _interopRequireDefault(_breadcrumbs);

var _ErrorReport = __webpack_require__(/*! ./ErrorReport */ "./sources/pages/notificationlist/components/ErrorReport.js");

var _ErrorReport2 = _interopRequireDefault(_ErrorReport);

__webpack_require__(/*! ../../../fonts/style.css */ "./sources/fonts/style.css");

__webpack_require__(/*! ./NotificationList.scss */ "./sources/pages/notificationlist/components/NotificationList.scss");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var NotificationList = function (_Component) {
  _inherits(NotificationList, _Component);

  function NotificationList(props) {
    _classCallCheck(this, NotificationList);

    var _this = _possibleConstructorReturn(this, (NotificationList.__proto__ || Object.getPrototypeOf(NotificationList)).call(this, props));

    _this.state = {
      isRemark: null
    };
    _this.toggleHandler = _this.toggleHandler.bind(_this);
    _this.updateRemark = _this.updateRemark.bind(_this);
    return _this;
  }

  _createClass(NotificationList, [{
    key: 'updateRemark',
    value: function updateRemark(event) {
      var remark = event.currentTarget.dataset.remark;

      this.setState({ isRemark: remark });
      this.props.toggleHandler();
    }
  }, {
    key: 'toggleHandler',
    value: function toggleHandler() {
      this.props.toggleHandler();
    }
  }, {
    key: 'render',
    value: function render() {
      return _react2.default.createElement(
        'div',
        null,
        _react2.default.createElement(_breadcrumbs2.default, { chatbotName: 'Skype Chatbot', pageTitle: 'Notification List' }),
        _react2.default.createElement(
          'table',
          { className: 'tbl w-100' },
          _react2.default.createElement(
            'thead',
            null,
            _react2.default.createElement(
              'tr',
              null,
              _react2.default.createElement(
                'th',
                { className: 'col-order' },
                '#'
              ),
              _react2.default.createElement(
                'th',
                null,
                'Sender/',
                _react2.default.createElement('br', null),
                ' Notification Type'
              ),
              _react2.default.createElement(
                'th',
                { className: 'col-send-date' },
                'Send Date/Time',
                _react2.default.createElement('br', null),
                ' (GMT+8)'
              ),
              _react2.default.createElement(
                'th',
                null,
                _react2.default.createElement(
                  'div',
                  { className: 'has-icons' },
                  'Title/Content ',
                  _react2.default.createElement('span', { className: 'icon-expand' })
                )
              ),
              _react2.default.createElement(
                'th',
                { className: 'col-status' },
                'Status'
              ),
              _react2.default.createElement(
                'th',
                { className: 'col-action' },
                'Action'
              )
            )
          ),
          _react2.default.createElement(
            'tbody',
            null,
            _react2.default.createElement(
              'tr',
              null,
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                '1'
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'div',
                  null,
                  'User1'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  'UM Notification'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement(
                  'div',
                  null,
                  '10:00:00 PM'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  '12/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'col-content' },
                _react2.default.createElement('span', { className: 'icon-down' }),
                'To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 ',
                _react2.default.createElement('br', null),
                _react2.default.createElement(
                  'strong',
                  null,
                  'WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018'
                ),
                ' ',
                _react2.default.createElement('br', null),
                'Good Day!',
                _react2.default.createElement('br', null),
                'This is Shane, kindly take note of the below message [\u8BF7\u6CE8\u610F\u4EE5\u4E0B\u516C\u544A]:',
                _react2.default.createElement('br', null),
                '>> All services subscribed with us including Sportsbook, Gaming and Back Office are not available during maintenance on going',
                _react2.default.createElement('br', null),
                '>> \u5728\u7EF4\u62A4\u671F\u95F4, \u6240\u6709\u548C\u6211\u4EEC\u8BA2\u9605\u7684\u670D\u52A1, \u5305\u62EC\u4F53\u80B2, \u5A31\u4E50\u5385\u548C\u540E\u53F0\u5C06\u65E0\u6CD5\u4F7F\u7528',
                _react2.default.createElement('br', null),
                'Thank you for your kind attention!',
                _react2.default.createElement('br', null)
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'span',
                  { className: 'badge-green' },
                  'Sent'
                )
              ),
              _react2.default.createElement('td', null)
            ),
            _react2.default.createElement(
              'tr',
              null,
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                '2'
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'div',
                  null,
                  'User1'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  'UM Notification'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement(
                  'div',
                  null,
                  '10:00:00 PM'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  '12/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'col-content' },
                _react2.default.createElement('span', { className: 'icon-right' }),
                'To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 ',
                _react2.default.createElement('br', null),
                _react2.default.createElement(
                  'strong',
                  null,
                  'WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'span',
                  { className: 'badge-blue' },
                  'Planned'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement('span', { className: 'icon-delete' })
              )
            ),
            _react2.default.createElement(
              'tr',
              null,
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                '3'
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'div',
                  null,
                  'User1'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  'UM Notification'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement(
                  'div',
                  null,
                  '10:00:00 PM'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  '12/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'col-content' },
                _react2.default.createElement('span', { className: 'icon-right' }),
                'To 4 recipient(s): ACBbet, XYZbet BBIN, ',
                _react2.default.createElement(
                  'span',
                  { className: 'text-red' },
                  'BBIN2, Gold Deluxe'
                ),
                ' ',
                _react2.default.createElement('br', null),
                _react2.default.createElement(
                  'strong',
                  null,
                  'WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'span',
                  { className: 'badge-red' },
                  'Error'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement('span', { className: 'icon-view', 'data-remark': true, onClick: this.updateRemark })
              )
            ),
            _react2.default.createElement(
              'tr',
              null,
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                '4'
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'div',
                  null,
                  'User1'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  'UM Notification'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement(
                  'div',
                  null,
                  '10:00:00 PM'
                ),
                _react2.default.createElement(
                  'div',
                  null,
                  '12/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'col-content' },
                _react2.default.createElement('span', { className: 'icon-right' }),
                'To 4 recipient(s): ACBbet, XYZbet BBIN, ',
                _react2.default.createElement(
                  'span',
                  { className: 'text-red' },
                  'BBIN2, Gold Deluxe'
                ),
                ' ',
                _react2.default.createElement('br', null),
                _react2.default.createElement(
                  'strong',
                  null,
                  'WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018'
                )
              ),
              _react2.default.createElement(
                'td',
                null,
                _react2.default.createElement(
                  'span',
                  { className: 'badge-orange' },
                  'Resolved'
                )
              ),
              _react2.default.createElement(
                'td',
                { className: 'text-center' },
                _react2.default.createElement('span', { className: 'icon-view', 'data-remark': false, onClick: this.updateRemark })
              )
            )
          )
        ),
        _react2.default.createElement(_ErrorReport2.default, {
          isModal: this.props.isModal,
          toggleHandler: this.toggleHandler,
          isRemark: this.state.isRemark })
      );
    }
  }]);

  return NotificationList;
}(_react.Component);

exports.default = (0, _reactRedux.connect)()(NotificationList);

/***/ }),

/***/ "./sources/pages/notificationlist/components/NotificationList.scss":
/*!*************************************************************************!*\
  !*** ./sources/pages/notificationlist/components/NotificationList.scss ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// extracted by mini-css-extract-plugin

/***/ }),

/***/ "./sources/pages/notificationlist/index.js":
/*!*************************************************!*\
  !*** ./sources/pages/notificationlist/index.js ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


__webpack_require__(/*! bootstrap/dist/css/bootstrap.css */ "./node_modules/bootstrap/dist/css/bootstrap.css");

var _react = __webpack_require__(/*! react */ "./node_modules/react/index.js");

var _react2 = _interopRequireDefault(_react);

var _reactDom = __webpack_require__(/*! react-dom */ "./node_modules/react-dom/index.js");

var _reactDom2 = _interopRequireDefault(_reactDom);

var _reactRedux = __webpack_require__(/*! react-redux */ "./node_modules/react-redux/es/index.js");

var _configureStore = __webpack_require__(/*! ./store/configureStore */ "./sources/pages/notificationlist/store/configureStore.js");

var _configureStore2 = _interopRequireDefault(_configureStore);

var _App = __webpack_require__(/*! ./App */ "./sources/pages/notificationlist/App.js");

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

/***/ "./sources/pages/notificationlist/store/configureStore.js":
/*!****************************************************************!*\
  !*** ./sources/pages/notificationlist/store/configureStore.js ***!
  \****************************************************************/
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
//# sourceMappingURL=notificationlist.chunk.js.map