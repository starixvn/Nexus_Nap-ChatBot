import React, { Component } from 'react';
import { connect } from 'react-redux';
import "../../../fonts/style.css";

import Sidebar from './Sidebar';
import Content from './Content';

import './Layout.scss';
class Layout extends Component {
    constructor(props){
        super(props);
        this.state = {
          expandedClass: "sidebar-expanded",
          currentUrl: "/NewNotification/Index",
          isSidebarExpanded: true
        }

        this.onMenuChange = this.onMenuChange.bind(this);
        this.collapseExpandHandler = this.collapseExpandHandler.bind(this);
      }

      onMenuChange(url){
        this.setState({
          currentUrl: url
        })
      }

      collapseExpandHandler(){
        this.setState({
          isSidebarExpanded: !this.state.isSidebarExpanded
        });
      }

      render() {
        return (
          <div className={`page ${this.state.isSidebarExpanded ? "sidebar-expanded" : ""}`}>    
            <Sidebar onChange={this.onMenuChange}></Sidebar>
            <Content contentUrl={this.state.currentUrl} collapseExpandHandler={this.collapseExpandHandler}></Content>
          </div>
        )
    }
}

export default connect()(Layout);
