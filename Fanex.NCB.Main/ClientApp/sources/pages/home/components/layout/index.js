import React from 'react';
import { connect } from 'react-redux';
import Sidebar from './../sidebar';
import Content from './../content';
import Site from '../../../lib/site';

import '../../../shared/components/font.css';
import './Layout.scss';

export class Layout extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      expandedClass: 'sidebar-expanded',
      currentUrl: Site.resolveClientUrl('NewNotification/Index'),
      isSidebarExpanded: true
    };
  }

  onMenuChange = url => {
    this.setState({
      currentUrl: url
    });
  }

  collapseExpandHandler = () => {
    this.setState({
      isSidebarExpanded: !this.state.isSidebarExpanded
    });
  }

  render() {
    return (
      <div className={`page ${this.state.isSidebarExpanded ? 'sidebar-expanded' : 'sidebar-collapsed'}`}>
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
