import React, { PureComponent } from 'react';
import ReactMarkdown from 'react-markdown/with-html';
import { defaultRendererOptions } from '../markdown-settings/default-markdown-options';
import '../markdown-settings/style.scss';

export default class TemplatePreview extends PureComponent {
    highlightParameters = text => {
        const params = [...new Set(text.match(/{[_-\w\s]+}/g))];// NOSONAR Make sure that using a regular expression is safe here
        let formattedText = text;

        for (let i = 0; i < params.length; i++) {
            formattedText = formattedText.replace(new RegExp(`\\${params[i]}`, 'g'), `<span style="color:red">${params[i]}</span>`);// NOSONAR Make sure that using a regular expression is safe here
        }

        return formattedText;
    };

    render() {
        return (
            <ReactMarkdown source={this.highlightParameters(this.props.text)} escapeHtml={false} linkTarget="blank"
                renderers={defaultRendererOptions}
            />
        );
    }
}