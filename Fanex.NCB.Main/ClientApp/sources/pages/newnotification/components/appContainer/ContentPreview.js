import React, { PureComponent } from 'react';
import remarkDisableTokenizers from "remark-disable-tokenizers";
import ReactMarkdown from 'react-markdown/with-html';

export default class ContentPreview extends PureComponent {
    highlightParameters = text => {
        text = text.replace(/(?:\r\n|\r|\n)/g, '<br>');
        return text;
    }

    plugins = [
        [
            remarkDisableTokenizers,
            {
                block: ['blockquote', 'indentedCode', 'fencedCode', 'table', 'atxHeading', 'list', 'footnote'],
                inline: ['code', 'reference']
            }
        ]
    ]

    render() {
        return (
            <ReactMarkdown source={this.highlightParameters(this.props.text)} escapeHtml={false} linkTarget="blank"
                plugins={this.plugins}
            />
        );
    }
}