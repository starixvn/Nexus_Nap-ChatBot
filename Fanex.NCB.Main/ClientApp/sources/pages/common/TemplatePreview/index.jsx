import React, { PureComponent } from 'react';
import remarkDisableTokenizers from "remark-disable-tokenizers";
import ReactMarkdown from 'react-markdown/with-html';

export default class TemplatePreview extends PureComponent {
    highlightParameters = text => {
        let params = [...new Set(text.match(/{[_-\w\s]+}/g))];

        if (params) {
            for (let i = 0; i < params.length; i++) {
                text = text.replace(new RegExp(params[i], 'g'), `<span style="color:red">${params[i]}</span>`);
            }
        }

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