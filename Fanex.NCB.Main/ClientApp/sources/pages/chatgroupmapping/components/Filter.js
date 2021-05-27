import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import MultiselectTreeView from '../../common/react-multiselect-treeview/MultiselectTreeView';
import Labels from '../../common/Labels';
import AutoComplete from '../../common/auto-complete';
import Site from '../../lib/site';
import { PAGING_SIZES } from '../../constants/constants';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            channelIds: '',
            botIds: '',
            recipientTypeIds: '',
            recipient: {
                value: ''
            },
            groupName: {
                value: ''
            },
            licenseeStatusIds: '',
            mappingStatusIds: '',
            settings: {
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
        if (prevState.settings.bots.length === 0 && nextProps.settings.bots) {
            const selectedChannelIds = [];
            const channels = [
                {
                    id: 1, text: 'Skype', isLeaf: true
                }, {
                    id: 2, text: 'Telegram', isLeaf: true
                }
            ].map(item => {
                selectedChannelIds.push(item['id']);
                return { ...item, checked: true };
            });

            const selectedbotIds = [];
            const bots = [
                {
                    id: 1, text: 'Skype', isLeaf: false,
                    items: [{ id: 1, text: 'Bot 1', isLeaf: true }, { id: 2, text: 'Bot 2', isLeaf: true }]
                }, {
                    id: 2, text: 'Telegram', isLeaf: false,
                    items: [{ id: 3, text: 'Bot 3', isLeaf: true }, { id: 4, text: 'Bot 4', isLeaf: true }]
                }
            ].map(item => {
                selectedbotIds.push(item['id']);
                return { ...item, checked: true };
            });

            const selectedRecipientTypeIds = [];
            const recipientTypes = nextProps.settings.recipientTypes.map(item => {
                selectedRecipientTypeIds.push(item['id']);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const selectedMappingStatusIds = [];
            const mappingStatuses = nextProps.settings.mappingStatuses.map(item => {
                selectedMappingStatusIds.push(item['id']);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const selectedlicenseeStatusIds = [];
            const licenseeStatuses = nextProps.settings.licenseeStatuses.map(item => {
                selectedlicenseeStatusIds.push(item.id);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const state = {
                channelIds: selectedChannelIds.join(','),
                botIds: selectedbotIds.join(','),
                recipientTypeIds: selectedRecipientTypeIds.join(','),
                recipient: {
                    value: ''
                },
                licenseeStatusIds: selectedlicenseeStatusIds.join(','),
                mappingStatusIds: selectedMappingStatusIds.join(','),
                settings: {
                    channels: channels,
                    bots: bots,
                    recipientTypes: recipientTypes,
                    mappingStatuses: mappingStatuses,
                    licenseeStatuses: licenseeStatuses
                }
            };

            return state;
        }

        return null;
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {
        return {
            channelIds: this.state.channelIds,
            botIds: this.state.botIds,
            recipientTypeIds: this.state.recipientTypeIds,
            recipientName: this.state.recipient.value,
            groupName: this.state.groupName.value,
            mappingStatusIds: this.state.mappingStatusIds,
            licenseeStatusIds: this.state.licenseeStatusIds,
            pageIndex: 1,
            pageSize: PAGING_SIZES[0]
        };
    }

    checkValidParams = () => {
        const { channelIds, botIds, recipientTypeIds, mappingStatusIds, licenseeStatusIds } = this.state;
        return channelIds != '' && botIds !== ''
            && recipientTypeIds !== '' && mappingStatusIds !== '' && licenseeStatusIds !== '';
    }

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    render() {
        return (
            <Form className="form-filter" tag='div' >
                <FormGroup>
                    <Label className="form-label">{Labels.RecipientType}</Label>
                    <MultiselectTreeView width={150}
                        data={this.state.settings.recipientTypes}
                        isSelectedAll
                        onChange={selectedItems => this.onChange('recipientTypeIds', selectedItems.map(item => item.id).join(','))}
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
                    <MultiselectTreeView width={150}
                        data={this.state.settings.licenseeStatuses}
                        isSelectedAll
                        onChange={selectedItems => this.onChange('licenseeStatusIds', selectedItems.map(item => item.id).join(','))}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.MappingStatus}</Label>
                    <MultiselectTreeView width={150}
                        data={this.state.settings.mappingStatuses}
                        isSelectedAll
                        onChange={selectedItems => this.onChange('mappingStatusIds', selectedItems.map(item => item.id).join(','))}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.BotService}</Label>
                    <MultiselectTreeView width={150}
                        data={this.state.settings.channels}
                        isSelectedAll
                        onChange={selectedItems => this.onChange('channelIds', selectedItems.map(item => item.id).join(','))}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.BotName}</Label>
                    <MultiselectTreeView width={150}
                        data={this.state.settings.bots}
                        isSelectedAll
                        onChange={selectedItems => this.onChange('botIds', selectedItems.map(item => item.id).join(','))}
                    />
                </FormGroup>
                <button type="submit" id="submit-button" disabled={!this.checkValidParams()} className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settings } = state;
    return {
        settings
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);