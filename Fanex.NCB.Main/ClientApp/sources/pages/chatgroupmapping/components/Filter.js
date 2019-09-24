import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { MultipleSelectField } from '../../common/MultipleSelectField';
import Labels from '../../common/Labels';
import AutoComplete from '../../common/auto-complete';
import Site from '../../lib/site';
import { PAGING_SIZES } from '../../constants/constants';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            botIds: {
                value: ''
            },
            recipientTypes: {
                value: ''
            },
            recipient: {
                value: ''
            },
            groupName: {
                value: ''
            },
            licenseeStatuses: {
                value: ''
            },
            mappingStatuses: {
                value: ''
            },
            settingData: {
                bots: [],
                recipientTypes: [],
                mappingStatuses: [],
                licenseeStatuses: []
            }
        };

        this.props.getSettings();
        this.props.filterRecipients([], 0);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (prevState.settingData.bots.length === 0 && nextProps.settingData.bots) {
            const bots = nextProps.settingData.bots;
            const selectedbotIds = [];
            bots.map(v => {
                v['checked'] = true;
                selectedbotIds.push(v['id']);
            });

            const recipientTypes = nextProps.settingData.recipientTypes;
            const selectedRecipientTypes = [];
            recipientTypes.map(v => {
                v['checked'] = true;
                selectedRecipientTypes.push(v['id']);
            });

            const mappingStatuses = nextProps.settingData.mappingStatuses;
            const selectedMappingStatuses = [];
            mappingStatuses.map(v => {
                v['checked'] = true;
                selectedMappingStatuses.push(v['id']);
            });

            const licenseeStatuses = nextProps.settingData.licenseeStatuses;
            const selectedlicenseeStatuses = [];
            licenseeStatuses.map(v => {
                v['checked'] = true;
                selectedlicenseeStatuses.push(v['id']);
            });

            const state = {
                botIds: {
                    value: selectedbotIds.join(',')
                },
                recipientTypes: {
                    value: selectedRecipientTypes.join(',')
                },
                recipient: {
                    value: ''
                },
                licenseeStatuses: {
                    value: selectedlicenseeStatuses.join(',')
                },
                mappingStatuses: {
                    value: selectedMappingStatuses.join(',')
                },
                settingData: {
                    bots: bots,
                    recipientTypes: recipientTypes,
                    mappingStatuses: mappingStatuses,
                    licenseeStatuses: licenseeStatuses
                }
            };

            return state;
        }

        // Return null to indicate no change to state.
        return null;
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {
        return {
            botIds: this.state.botIds.value,
            recipientTypeIds: this.state.recipientTypes.value,
            recipientName: this.state.recipient.value,
            groupName: this.state.groupName.value,
            mappingStatusIds: this.state.mappingStatuses.value,
            licenseeStatusIds: this.state.licenseeStatuses.value,
            pageIndex: 1,
            pageSize: PAGING_SIZES[0]
        };
    }

    checkValidParams = () => this.state.botIds.value !== '' &&
        this.state.recipientTypes.value !== '' &&
        this.state.mappingStatuses.value !== '' &&
        this.state.licenseeStatuses.value !== '';

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    render() {
        return (
            <Form className="form-filter" >
                <FormGroup>
                    <Label className="form-label">{Labels.RecipientType}</Label>
                    <MultipleSelectField
                        id={'recipientTypes'}
                        width={150}
                        initialValue={this.state.recipientTypes}
                        data={this.state.settingData.recipientTypes}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('recipientTypes', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.Recipient}</Label>
                    <AutoComplete
                        url={Site.resolveClientUrl('ChatGroupMapping/SearchRecipient')}
                        onSelect={value => this.onChange('recipient', { value: value })}
                        onInput={value => this.onChange('recipient', { value: value })}
                        htmlAttributes={{ maxLength: 50 }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.GroupName}</Label>
                    <AutoComplete
                        url={Site.resolveClientUrl('ChatGroupMapping/SearchChatGroup')}
                        onSelect={value => this.onChange('groupName', { value: value })}
                        onInput={value => this.onChange('groupName', { value: value })}
                        htmlAttributes={{ maxLength: 50 }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.LicenseeStatus}</Label>
                    <MultipleSelectField
                        id={'licenseeStatuses'}
                        width={150}
                        initialValue={this.state.licenseeStatuses}
                        data={this.state.settingData.licenseeStatuses}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('licenseeStatuses', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.MappingStatus}</Label>
                    <MultipleSelectField
                        id={'mappingStatuses'}
                        width={150}
                        initialValue={this.state.mappingStatuses}
                        data={this.state.settingData.mappingStatuses}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('mappingStatuses', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.BotName}</Label>
                    <MultipleSelectField
                        id={'botIds'}
                        width={150}
                        initialValue={this.state.botIds}
                        data={this.state.settingData.bots}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('botIds', value)}
                    />
                </FormGroup>
                <button type="submit" id="submit-button" disabled={!this.checkValidParams()} className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settingData } = state;
    return {
        settingData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);