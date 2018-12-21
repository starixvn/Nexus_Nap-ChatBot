import React, { Component } from 'react';
import { connect } from 'react-redux';

import './Header.scss';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';

class Header extends Component {
    constructor(props){
        super(props);
        
        this.state = {
          dropdownOpen: false
        };

        this.collapseExpandHandler = this.collapseExpandHandler.bind(this);
        this.toggle = this.toggle.bind(this);
      }
    
      toggle() {
        this.setState(prevState => ({
          dropdownOpen: !prevState.dropdownOpen
        }));
      }

      collapseExpandHandler(){
        this.props.collapseExpandHandler();
      }
    
      render() {
        return (
            <div className="header">
               <div class="line-wrapp" onClick={this.collapseExpandHandler}>
                    <div class="line top"></div>
                    <div class="line center"></div>
                    <div class="line bottom"></div>
                </div>
                <ul className="list-unstyled userinfo">
                        <li className="annoucement">
                            <div className="icon-bell"></div>
                            <div className="pulse-css"></div>
                            <div className="number">99+</div>
                        </li>
                        <li className="user-info">
                        <Dropdown isOpen={this.state.dropdownOpen} toggle={this.toggle}>
                            <DropdownToggle 
                                tag="a">
                                <div className="profile-picture">
                                    <span className="icon-username"></span>
                                </div>
                                Welcome Sa
                            </DropdownToggle>
                            <DropdownMenu>
                                <DropdownItem tag="a" href="#">
                                    <span className="icon-change-pass"></span>Change Password
                                </DropdownItem>
                                <DropdownItem tag="a" href="/Auth/Index">
                                    <span className="icon-sign-out"></span>Sign Out
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                        </li>
                    </ul>
            </div>
        )
    }
}

export default connect()(Header);
