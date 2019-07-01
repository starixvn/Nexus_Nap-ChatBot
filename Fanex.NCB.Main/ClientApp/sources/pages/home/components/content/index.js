import React from 'react';
import { connect } from 'react-redux';
import Header from './../header';

import './Content.scss';

export class Content extends React.PureComponent {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <div className="main-container">
            <Header
                collapseExpandHandler={this.props.collapseExpandHandler}
                isSidebarExpanded={this.props.isSidebarExpanded}
            />
            <div className="content">
                <iframe className="main-content" src={this.props.contentUrl} />
            </div>
        </div>
    );
  }
}

export default connect()(Content);