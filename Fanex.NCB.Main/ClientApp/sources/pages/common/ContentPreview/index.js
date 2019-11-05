import React, { PureComponent } from 'react';
import ReactMarkdown from 'react-markdown/with-html';
import { defaultRendererOptions } from '../markdown-settings/default-markdown-options';
import '../markdown-settings/style.scss';

export default class ContentPreview extends PureComponent {
    render() {
        return (
            <ReactMarkdown source={this.props.text} linkTarget="blank" renderers={defaultRendererOptions}
                escapeHtml={false}
            />
        );
    }
}