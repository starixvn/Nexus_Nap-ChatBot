import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Button } from 'reactstrap';
import { Container, Row, Col } from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import './AppContainer.scss';
import Labels from '../../common/Labels';
import { SelectField } from '../../common/SelectField';
import { TextField } from '../../common/text-field/TextField';
import { InputField } from '../../common/InputField';
import { TemplateTextField } from '../../common/template-text-field/TemplateTextField';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isSummited: false,
            template: {
                title: {
                    originTemplate: "",
                    middleTemplate: "",
                    processedTemplate: ""
                    
                },
                content: {
                    originTemplate: "",
                    middleTemplate: "",
                    processedTemplate: ""                    
                },
                params: []
            }            
        };

        this.props.getSettings();
        this.props.getTemplateTypes();
        
    }

    onChange = (fieldName, fieldValue) => this.setState({ [fieldName]: fieldValue, isChanged: true });    

    detectParams = (text) => {
        let paramRegex = /{(.+)}/g;
        let params = [];        

        let copiedText = text.slice(0);
        let match = paramRegex.exec(copiedText);
        while (match) {
            let matchText = match[0];
            let regex = new RegExp(matchText, "g");

            console.log("copiedtext");
            console.log(matchText);
            console.log(copiedText);
            params.push({ name: matchText, value: { value: matchText, isValid: false } });            
            copiedText = copiedText.replace(regex, "");
            match = paramRegex.exec(copiedText);
        }

        return params;
    }

    findMatches = (regex, str, matches = []) => {
        const res = regex.exec(str);
        res && matches.push(res);// && this.findMatches(regex, str, matches);

        return matches;
    }

    createMiddleTemplate = (text) => {
        return text.replace(/{/g, '<span class="red">{').replace(/}/g, '}</span>');
    }

    getTemplate = () => {
        this.props.getTemplate().then(() => {
            console.log("template");
            console.log(this.props);

            let titleTemplate = this.props.template.title;            
            let titleMiddleTemplate = this.createMiddleTemplate(titleTemplate);  
            let contentTemplate = this.props.template.content;
            let contentMiddleTemplate = this.createMiddleTemplate(contentTemplate);          
            let params = this.detectParams(titleTemplate + contentTemplate);

            this.setState(
                {
                    template: {
                        title: {
                            originTemplate: titleTemplate,
                            middleTemplate: titleMiddleTemplate,
                            processedTemplate: titleMiddleTemplate                            
                        },
                        content: {
                            originTemplate: contentTemplate,
                            middleTemplate: contentMiddleTemplate,
                            processedTemplate: contentMiddleTemplate                            
                        },
                        params: params
                    }
                }
            );
        });
    }

    splitArrayIntoTwoColumns = (array) => {
        let newArray = [];
        let length = array.length;
        for (let i = 0; i < length; i += 2) {
            newArray.push(array.slice(i, i + 2));
        }

        return newArray;
    }

    processTemplate = (text, params) => {
        let copiedText = text.slice(0);
        params.map((item) => {
            let regex = new RegExp(item.name, "g");
            copiedText = copiedText.replace(regex, item.value.value);            
        });

        return copiedText;
    }

    updateParam = (name, value) => {
        let params = this.state.template.params.slice(0);
        for (let i = 0; i < params.length; i++) {
            if (params[i].name === name) {
                params[i].value = value;
                break;
            }
        }

        return params;
    }

    updateTemplate = (name, value) => {
        let params = this.updateParam(name, value);
        let titleProcessedTemplate = this.processTemplate(this.state.template.title.middleTemplate, params);
        let contentProcessedTemplate = this.processTemplate(this.state.template.content.middleTemplate, params);

        this.setState(
            prevState => ({
                ...prevState,
                template: {
                    title: {
                        ...prevState.template.title,
                        processedTemplate: titleProcessedTemplate
                    },
                    content: {
                        ...prevState.template.content,
                        processedTemplate: contentProcessedTemplate
                    },
                    params: params
                }
            })
        );
    }

    onChange = (name, value) => {
        this.updateTemplate(name, value);
    };

    render() {        
        if (!this.props.settingData.notificationTypes) {
            return null;
        }

        let splittedParams = this.splitArrayIntoTwoColumns(this.state.template.params);

        console.log("render");
        //console.log(splittedParams);

        //console.log(this.props);
        console.log(this.state);

        
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form">
                    <Form>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Notification Type</Label>
                            <SelectField 
                                data={this.props.settingData.notificationTypes}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Product</Label>
                            <SelectField
                                data={this.props.settingData.products}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Notification Template</Label>
                            <SelectField
                                data={this.props.notificationTemplates}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.getTemplate(notificationTypeId); this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Title</Label>
                            <TemplateTextField
                                value={this.state.template.title.processedTemplate}
                            />                                  
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Content</Label>
                            <TemplateTextField
                                value={this.state.template.content.processedTemplate}
                            />                                                                  
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">&nbsp;</Label>
                            <div>
                                <div>Parameters</div>
                                <Container>
                                    {
                                        splittedParams.map((items, rowIndex) => {
                                            return <Row key={rowIndex}>
                                                {
                                                    items.map((item, colIndex) => {
                                                        console.log(item);

                                                        return <Col key={colIndex}>
                                                            <Label className="form-label">{item.name}</Label>
                                                            <InputField initialValue={item.value.value}
                                                                isSummited={this.state.isSummited}
                                                                onChange={(value) => this.onChange(item.name, value)}
                                                            />
                                                        </Col>;
                                                    })
                                                }
                                            </Row>;
                                        })
                                    }
                                    
                                </Container>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Send Date/Time</Label>

                        </FormGroup>

                        <Button className="btn-submit">Save</Button>

                    </Form>
                </div>
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData, notificationTemplates, template  } = state;
    return Object.assign({}, { settingData, notificationTemplates, template });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);