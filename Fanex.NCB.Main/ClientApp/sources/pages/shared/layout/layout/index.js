import React from 'react';
import ReactDOM from 'react-dom';

import 'bootstrap/dist/css/bootstrap.css';
import '../../components/font.css';
import './Layout.scss';

import Sidebar from './../sidebar';
import Content from './../content';
import Site from '../../../lib/site';
import AuthRecheck from './../recheckin';

class Layout extends React.PureComponent {
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
                >
                    {this.props.children}
                </Content>
                <AuthRecheck />
            </div>
        );
    }
}

export function renderLayout(content) {
    ReactDOM.render(
        <Layout>
            {content}
        </Layout>, document.getElementById('root'));
}