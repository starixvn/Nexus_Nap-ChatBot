import React, { Component } from 'react';
import ReactMarkdown from 'react-markdown/with-html';
import { defaultRendererOptions } from '../markdown-settings/default-markdown-options';

export class PanelContent extends Component {

    constructor(props) {
        super(props);

        this.state = {
            isOpen: false
        };
    }

    handleToggle = () => this.setState(prevState => ({ isOpen: !prevState.isOpen }));

    handleSet = isOpen => this.setState({ isOpen: isOpen });

    render() {
        return (
            <div className={`nap-panel ${this.state.isOpen ? '' : 'nap-line'}`}>
                <div className="panel-header" onClick={this.handleToggle}>
                    <ReactMarkdown source={this.props.content} linkTarget="blank" renderers={defaultRendererOptions}
                        escapeHtml={false}
                    />
                    <span className={`icon-right ${this.state.isOpen ? 'content-expand' : 'content-collapse'}`} />
                </div>
            </div>
        )
    }
}

export default PanelContent;