import React, { Component } from 'react';
import { connect } from 'react-redux';
import Sidebar from './Sidebar';
import Content from './Content';
import Site from '../../lib/site';

import "../../shared/components/font.css";
import './Layout.scss';

class Layout extends Component {
    constructor(props){
        super(props);
        this.state = {
            expandedClass: "sidebar-expanded",
            currentUrl: Site.resolveClientUrl("NewNotification/Index"),
            isSidebarExpanded: true
        };

        this.onMenuChange = this.onMenuChange.bind(this);
        this.collapseExpandHandler = this.collapseExpandHandler.bind(this);
      }

      onMenuChange(url){
        this.setState({
          currentUrl: url
        });
      }

      collapseExpandHandler(){
        this.setState({
          isSidebarExpanded: !this.state.isSidebarExpanded
        });
      }

      render() {
        return (
          <div className={`page ${this.state.isSidebarExpanded ? "sidebar-expanded" : "sidebar-collapsed"}`}>    
            <Sidebar onChange={this.onMenuChange} />
            <Content 
                contentUrl={this.state.currentUrl}
                isSidebarExpanded={this.state.isSidebarExpanded}
                collapseExpandHandler={this.collapseExpandHandler}
            />
          </div>
        );
    }
}

export default connect()(Layout);
