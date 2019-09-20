import React from 'react';
import {UpdatePopup} from '../UpdatePopup';
import ShallowRenderer from 'react-test-renderer/shallow';
import Adapter from 'enzyme-adapter-react-16';
import configureMockStore from 'redux-mock-store';
import { shallow, render, configure } from 'enzyme';

describe('UpdatePopup', () => {
    const defaultProps = {
        updatePopupData: {
            editingItem: {
                notificationTypeName: {},
                recipientTypeIds: {},
                chatGroupTypeIds: {},
                systemId:{}
            }
        }
    };

    describe("constructor", () => {
        it("init state", () => {
            // Arrange           
            let props = {
                updatePopupData: {
                    editingItem: {
                        notificationTypeId: 1,
                        notificationTypeName: {value: "abc", isValid: true},
                        recipientTypeIds: { value: 2, isValid: true },
                        chatGroupTypeIds: { value: 2, isValid: true },
                        systemId: {value: 3, isValid: true}
                    }
                }
            };            

            // Act
            let updatePopup = new UpdatePopup(props);

            // Assert
            expect(updatePopup.state.notificationTypeId).toEqual(1);
            expect(updatePopup.state.notificationTypeName.value).toEqual("abc");
            expect(updatePopup.state.notificationTypeName.isValid).toBe(true);
            expect(updatePopup.state.recipientTypeIds.value).toEqual(2);
            expect(updatePopup.state.recipientTypeIds.isValid).toBe(true);
            expect(updatePopup.state.systemId.value).toEqual(3);
            expect(updatePopup.state.systemId.isValid).toBe(true);
            expect(updatePopup.state.isSummited).toBe(false);
        });
    });

    describe("mount", () => {
        it("componentWillMount", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    editingItem: {
                        notificationTypeName: {},
                        notificationTypeId: {},
                        recipientTypeIds: {},
                        chatGroupTypeIds: {},
                        systemId:{}
                    }
                },
                getSettings: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            updatePopup.componentWillMount();

            // Assert
            expect(props.getSettings).toHaveBeenCalled();
        });
    });

    describe("submitHandler", () => {        

        it("call prevenDefault", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.setState = jest.fn();
            let event = {preventDefault: jest.fn()};
            
            // Act
            updatePopup.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
        });

        it("when isSummited is false - set isSummited to true", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state.isSummited = false;
            updatePopup.setState = jest.fn();
            
            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({isSummited: true});
        });

        it("when isSummited is false - not update isSummited", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state.isSummited = true;
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(updatePopup.setState).not.toHaveBeenCalledWith({isSummited: true});
        });

        it("when valid params - call props.update then call closePopup", (done) => {
             // Arrange    
             let response = {};
             let props =  {
                ...defaultProps,
                update: jest.fn(() => Promise.resolve(response))
            };
             let updatePopup = new UpdatePopup(props);

             let params = {data1: []};
             updatePopup.setState = jest.fn();
             updatePopup.closePopup = jest.fn();
             updatePopup.getParams = jest.fn(() => {return params;});
             updatePopup.checkValidParams = jest.fn(() => {return true;});
 
             // Act
             updatePopup.submitHandler({preventDefault: jest.fn()});

             // Assert
             expect(props.update).toHaveBeenCalledWith(params, props.updatePopupData.isNew);
             props.update().then(() => {
                expect(updatePopup.closePopup).toHaveBeenCalled();
                done();
             });
        });

        it("when not valid params - do not call props.update", () => {
            // Arrange    
            let response = {};
            let props =  {
               ...defaultProps,
               update: jest.fn(() => Promise.resolve(response))
           };
            let updatePopup = new UpdatePopup(props);
            
            updatePopup.setState = jest.fn();
            updatePopup.checkValidParams = jest.fn(() => {return false;});
           
            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(props.update).not.toHaveBeenCalled();
        });
    });

    describe("get Params", () => {
        it("get params from state", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeId: 1,
                notificationTypeName: {value: "some name"},
                recipientTypeIds: { value: "2" },
                chatGroupTypeIds: { value: "4" },
                systemId: { value: 3 }                
            };
            let params = {
                notificationTypeId: 1,
                notificationTypeName: "some name",
                recipientTypeIds: "2",
                chatGroupTypeIds: "4",
                systemId: 3
            };

            // Act
            let result = updatePopup.getParams();

            // Assert
            expect(result).toEqual(params);
        });
    });

    describe("checkValidParams", () => {
        it("valid params", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: { isValid: true },
                chatGroupTypeIds: { isValid: true },
                systemId: {isValid: true}
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(true);
        });

        it("invalid notificationTypeName - return invalid", () => {
             // Arrange
             let updatePopup = new UpdatePopup(defaultProps);
             updatePopup.state = {
                 isSummited: true,
                 notificationTypeName: {isValid: false},
                 recipientTypeIds: { isValid: true },
                 chatGroupTypeIds: { isValid: true },
                 systemId: {isValid: true}
             };

             // Act
             let result = updatePopup.checkValidParams();

             // Assert
             expect(result).toBe(false);
        });

        it("invalid recipientTypeId - return invalid", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: { isValid: false },
                chatGroupTypeIds: { isValid: true },
                systemId: {isValid: true}
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });

        it("invalid chatGroupTypeId - return invalid", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: { isValid: true },
                recipientTypeIds: { isValid: true },
                chatGroupTypeIds: { isValid: false },
                systemId: { isValid: true }
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });

        it("invalid systemId - return invalid", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: { isValid: true },
                chatGroupTypeIds: { isValid: true },
                systemId: {isValid: false}
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });
    });

    describe("onChange", () => {
        it("when called - setState", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            let fieldName = "field name";
            let fieldValue = 123;
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.onChange(fieldName, fieldValue);

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({[fieldName]: fieldValue});
        });
    });

    describe("closePopup", () => {
        it("when addedResult is success - call closeUpdatePopupHandler after 2000ms", (done) => {
            // Arrange
            let props = {
                ...defaultProps,
                updateData: {
                    addedResult: { isSuccessful: true}
                },
                closeUpdatePopupHandler: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            updatePopup.closePopup();

            // Assert
            setTimeout(() => {
                expect(props.closeUpdatePopupHandler).toHaveBeenCalled();
                done();
            }, 2100);
        });

        it("when addedResult is NOT success - do not call closeUpdatePopupHandler after 2000ms", (done) => {
            // Arrange
            let props = {
                ...defaultProps,
                updateData: {
                    addedResult: { isSuccessful: false }
                },
                closeUpdatePopupHandler: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            updatePopup.closePopup();

            // Assert
            setTimeout(() => {
                expect(props.closeUpdatePopupHandler).not.toHaveBeenCalled();
                done();
            }, 2100);
        });
    });

    describe("closeConfirmation", () => {
        it("closeConfirmation  - set isShowedConfirm to false", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.closeConfirmation();

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({
                isShowedConfirm: false
            });
        });
    });

    describe("togglePopup ", () => {
        it("togglePopup   - set isShowedConfirm to true", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.togglePopup ();

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({
                isShowedConfirm: true
            });
        });
    });

    describe("updateDropdownData", () => {
        it("updateDropdownData - update selected items", () => {
            // Arrange
            let items = [
                { id: 1, checked: true },
                { id: 2, checked: false },
                { id: 3, checked: true }
            ];
            let selectedIds = "2,3";
            let expectedItems = [
                { id: 1, checked: false },
                { id: 2, checked: true },
                { id: 3, checked: true }
            ];
            let updatePopup = new UpdatePopup(defaultProps);

            // Act
            updatePopup.updateDropdownData(items, selectedIds);

            // Assert
            expect(items).toEqual(expectedItems);
        });
    });

    describe("render", () => {
        const defaultProps = {
            updatePopupData: {
                editingItem: {
                    notificationTypeName: { value: "abc", isValid: true },
                    recipientTypeIds: { value: "2", isValid: true },
                    chatGroupTypeIds: { value: "2", isValid: true },
                    systemId:{ value: 3, isValid: true }
                },
                popupTitle: "title",                    
                successMsg: "sucess"                   
            },
            closeUpdatePopupHandler: jest.fn(),
            getSettings: jest.fn(),
            settingData: {
                recipientTypes: [],
                notificationSystems: []
            }
        };
        const mockStore = configureMockStore();
        configure({ adapter: new Adapter() });
        

        it("when has addedResult - show Notification", () => {
            // Arrange
            let props = {
                ...defaultProps,
                updateData: {
                    addedresult: true
                }
            };    

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when not have addedResult - do not show Notification", () => {
             // Arrange
             let props = {
                ...defaultProps,
                updateData: {
                    addedresult: null
                }
            };    

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when notificationTypeName inputfield changed, call onChange on notificationTypeName", () => {
            // Arrange
            const initialState = { settingData: { data1: [] }, updatePopupData: { data2: [] }, updateData: { data3: [] } };
            let props = {
                ...defaultProps,
                updateData: {
                    addedresult: null
                }
            };    
            let store = mockStore(initialState);

            let wrapper = shallow(
                <UpdatePopup store={store} {...props} />
            );

            wrapper.instance().onChange = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            const event = { target: { name: "aa", value: "xx" } };
            wrapper.find('InputField').first().simulate('change', event);

            // Assert
            expect(wrapper.instance().onChange).toHaveBeenCalledWith('notificationTypeName', event);
        });

        it("when recipientType field changed, call onChange on recipientTypeIds", () => {
            // Arrange
            const initialState = { settingData: { data1: [] }, updatePopupData: { data2: [] }, updateData: { data3: [] } };
            let props = {
                ...defaultProps,
                updateData: {
                    addedresult: null
                }
            };
            let store = mockStore(initialState);

            let wrapper = shallow(
                <UpdatePopup store={store} {...props} />
            );

            wrapper.instance().onChange = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            const event = { target: { name: "aa", value: "xx" } };
            wrapper.find('MultipleSelectField').first().simulate('change', event);

            // Assert
            expect(wrapper.instance().onChange).toHaveBeenCalledWith('recipientTypeIds', event);
        });

        it("when chatGroupType field changed, call onChange on chatGroupTypeIds", () => {
            // Arrange
            const initialState = { settingData: { data1: [] }, updatePopupData: { data2: [] }, updateData: { data3: [] } };
            let props = {
                ...defaultProps,
                updateData: {
                    addedresult: null
                }
            };
            let store = mockStore(initialState);

            let wrapper = shallow(
                <UpdatePopup store={store} {...props} />
            );

            wrapper.instance().onChange = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            const event = { target: { name: "aa", value: "xx" } };
            wrapper.find('MultipleSelectField').last().simulate('change', event);

            // Assert
            expect(wrapper.instance().onChange).toHaveBeenCalledWith('chatGroupTypeIds', event);
        });

        it("when system field changed, call onChange on systemId", () => {
            // Arrange
            const initialState = { settingData: { data1: [] }, updatePopupData: { data2: [] }, updateData: { data3: [] } };
            let props = {
                ...defaultProps,
                updateData: {
                    addedresult: null
                }
            };
            let store = mockStore(initialState);

            let wrapper = shallow(
                <UpdatePopup store={store} {...props} />
            );

            wrapper.instance().onChange = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            const event = { target: { name: "aa", value: "xx" } };
            wrapper.find('SelectField').last().simulate('change', event);

            // Assert
            expect(wrapper.instance().onChange).toHaveBeenCalledWith('systemId', event);
        });
    });
});