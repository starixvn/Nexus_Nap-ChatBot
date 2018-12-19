import React, {Component} from 'react';
import { connect } from 'react-redux';
import './Menu.scss';

class Menu extends Component {
    constructor(props){
        super(props);

        this.onChange = this.onChange.bind(this);
    }

    onChange(url){
        this.props.onChange(url);
    }

    render(){
        return (
            <div role="tabpanel" className="leftmenu-tab" data-ng-cloak>
              <ul className="nav nav-tabs" role="tablist">
                  <li role="presentation" className="active">
                      <div role="tab" data-toggle="tab" data-target="#tab-menu">Menu</div>
                  </li>       
              </ul>
              <div className="tab-content">
                  <div role="tabpanel" className="menu-tab-content tab-pane active" id="tab-menu">
                      <ul className="list-unstyled">                
                          <li className="menu-item" data-ng-repeat="menu in vm.menus">
                              <div className="menu-item-content" data-ng-click="vm.menuClick($index);">
                                  <span className="icon-client"></span>
                                  <span className="menu-title" data-ng-bind="::menu.Name">Skype Chatbot</span>
                                  <span className="pull-right" data-ng-class="icon-chevron-down"></span>
                              </div>
                              <ul className="list-unstyled b-ul__submenu" data-ng-hide="!menu.selected && vm.expandAsideMenu">                                        
                                  <li className="submenu-item" data-ng-class="{active: submenu.selected}"
                                      data-ng-repeat="submenu in menu.SubMenus">
                                      <div className="submenu-item-content" data-ng-click="vm.subMenuClick($parent.$index, $index)">
                                          <div onClick={() => this.onChange("/NewNotification/Index")}>New Notification</div>
                                      </div>
                                  </li>
                                    <li className="submenu-item" data-ng-class="{active: submenu.selected}"
                                      data-ng-repeat="submenu in menu.SubMenus">
                                      <div className="submenu-item-content" data-ng-click="vm.subMenuClick($parent.$index, $index)">
                                          <div onClick={() => this.onChange("/NotificationList/Index")}>Notification List</div>
                                      </div>
                                  </li>
                                  <li className="submenu-item" data-ng-class="{active: submenu.selected}"
                                      data-ng-repeat="submenu in menu.SubMenus">
                                      <div className="submenu-item-content" data-ng-click="vm.subMenuClick($parent.$index, $index)">
                                          <div onClick={() => this.onChange("/NotificationTemplate/Index")}>Notification Template</div>
                                      </div>
                                  </li>
                                  <li className="submenu-item" data-ng-class="{active: submenu.selected}"
                                      data-ng-repeat="submenu in menu.SubMenus">
                                      <div className="submenu-item-content" data-ng-click="vm.subMenuClick($parent.$index, $index)">
                                          <div onClick={() => this.onChange("/ChatGroupMapping/Index")}>Chat Group Mapping</div>
                                      </div>
                                  </li>
                                  <li className="submenu-item" data-ng-class="{active: submenu.selected}"
                                      data-ng-repeat="submenu in menu.SubMenus">
                                      <div className="submenu-item-content" data-ng-click="vm.subMenuClick($parent.$index, $index)">
                                          <div onClick={() => this.onChange("/NotificationType/Index")}>Notification Type</div>
                                      </div>
                                  </li>
                              </ul>
                          </li>                
                      </ul>
                  </div>       
              </div>
            </div>
          );
    }
}

export default connect()(Menu);
