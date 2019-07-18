import React from 'react';
import Header from './../header';

import './Content.scss';

export default class Content extends React.PureComponent {
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
                    {this.props.children}
                </div>
            </div>
        );
    }
}