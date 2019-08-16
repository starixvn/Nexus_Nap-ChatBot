import React, { Component } from 'react';
import { Collapse } from 'reactstrap';
import './Panel.scss';

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
                <div className="panel-header" onClick={this.toggle}>
                    {this.props.children[0]}

                    <span className={`icon-right ${this.state.isOpen ? 'content-expand' : 'content-collapse'}`} />
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