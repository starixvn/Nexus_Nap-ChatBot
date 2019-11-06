import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';

import './style.scss';

export default class MarkdownRules extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const exampleUrl = 'http://google.com/';
        return (
            <Modal isOpen={this.props.show} toggle={this.props.onToggle} wrapClassName="popup-markdown-rules" backdrop="static">
                <ModalHeader toggle={this.props.onToggle}>Markdown Rules</ModalHeader>
                <ModalBody>
                    <p className="info">
                        1. Do not support markdown in multiple paragraphs, each one must be marked down separately.
                         <br />
                        Ex:
                        <ul>
                            <li>Bold Text markdown **Text** treat as <strong>Text</strong></li>
                            <li>Italic Text markdown *Text* treat as <em>Text</em></li>
                            <li>Strikethrough Text markdown ~~Text~~ treat as <del>Text</del></li>
                            <li>Hyperlink Google markdown: [Google]({exampleUrl}) treat as <a target="blank" href={exampleUrl}>Google</a></li>
                        </ul>

                        2. Some markdowns similar to html code may lead to text missing or unexpected parsing. Please use \ before the markdown to escape markdown parsing.
                         <br />
                        Ex: \**Text** treat as **Text**
                         <br />
                        &lt;\!Good Day!\&gt; treat as &lt;!Good Day!&gt;
                    </p>
                </ModalBody>
            </Modal>
        );
    }
}