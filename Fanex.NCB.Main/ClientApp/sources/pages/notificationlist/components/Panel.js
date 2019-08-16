import React, { Component } from 'react';
import { Collapse } from 'reactstrap';

export default class Panel extends Component {
    static defaultProps = {
        defaultOpened: false
    }

    constructor(props) {
        super(props);

        this.state = {
            isOpen: props.defaultOpened
        };
    }

    toggle = () => this.setState(prevState => ({ isOpen: !prevState.isOpen }))

    set = isOpen => this.setState({ isOpen: isOpen });

    render() {
        return (
            <div className="nap-panel">
                <div className={`panel-header ${this.state.isOpen ? 'expand' : 'collapse'}`} onClick={this.toggle}>
                    {this.props.children[0]}
                </div>
                <Collapse isOpen={this.state.isOpen}>
                    {this.props.children[1]}
                </Collapse>
            </div>
        );
    }
}

export function PanelHeader(props) {
    return (
        <>
            {props.children}
        </>
    );
}