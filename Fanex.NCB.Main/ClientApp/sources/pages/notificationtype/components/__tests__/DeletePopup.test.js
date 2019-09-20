import React from 'react';
import {DeletePopup} from '../DeletePopup';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';
import Adapter from 'enzyme-adapter-react-16';
import configureMockStore from 'redux-mock-store';
import { shallow, render, configure } from 'enzyme';

describe('DeletePopup', () => {   
    describe('confirmHandler', () => {
        it('confirmHandler - call deleteItem', (done) => {
           // Arrange
           const response = { data: [] };
           let props = {
               deleteItem: jest.fn((id) => Promise.resolve(response)),
               closeDeletePopupHandler: jest.fn(),
               deletePopupData: { notificationTypeId: 1}
           };
            let deletePopup = new DeletePopup(props);
            deletePopup.closePopup = jest.fn();

           // Act
           deletePopup.confirmHandler();

           // Assert
           expect(props.deleteItem).toHaveBeenCalledWith(1);           
           props.deleteItem().then(() =>{
               expect(deletePopup.closePopup).toHaveBeenCalled();
            done();
           });
        });       
    });

    describe("cancelHandler", () => {
        it('cancelHandler - call close popup', () => {
            // Arrange
            let props = {
                deleteItem: jest.fn(),
                closeDeletePopupHandler: jest.fn()
            };
            let deletePopup = new DeletePopup(props);

            // Act
            deletePopup.cancelHandler();

            // Assert
            expect(props.closeDeletePopupHandler).toHaveBeenCalled();
        });
    });

    describe('closePopup', () => {
        it('when is not successful - do not call close Popup', (done) => {
            // Arrange
            let props = {
                deleteData: {
                    deleteResult: {
                        isSuccessful: false
                    }
                },
                closeDeletePopupHandler: jest.fn()
            };
            let deletePopup = new DeletePopup(props);

            // Act
            deletePopup.closePopup();

            // Asssert
            setTimeout(() => {
                expect(props.closeDeletePopupHandler).not.toHaveBeenCalled();
                done();
            }, 1100);
        });

        it('when is successful - close Popup', (done) => {
            // Arrange
            let props = {
                deleteData: {
                    deleteResult: {
                        isSuccessful: true
                    }
                },
                closeDeletePopupHandler: jest.fn()
            };
            let deletePopup = new DeletePopup(props);

            // Act
            deletePopup.closePopup();

            // Asssert
            setTimeout(() => {
                expect(props.closeDeletePopupHandler).toHaveBeenCalled();
                done();
            }, 1100);
        });
    });

    describe("render", () => {
        const mockStore = configureMockStore();
        configure({ adapter: new Adapter() });

        it("when no deleteData - return null", () => {
            // Arrange
            let props = {
                deletePopupData: {
                    content: "some content"
                },
                deleteData: { deleteResult: null },
                closeDeletePopupHandler: jest.fn()
            };

            // Act
            let result = (new ShallowRenderer()).render(<DeletePopup {...props} />);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when has deleteData", () => {
            // Arrange
            let props = {
                deletePopupData: {
                    content: "some content"
                },
                deleteData: {
                    deleteResult: {
                        isSuccessful: true,
                        successMsg: "msg1",
                        message: "msg2"
                    }
                },
                closeDeletePopupHandler: jest.fn()
            };
          
            // Act
            let result = (new ShallowRenderer()).render(<DeletePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when click OK - call confirmHandler", () => {
            // Arrange
            const initialState = { deletePopupData: { content: "some content" } };
            let props = {
                deleteData: {                    
                },
                deletePopupData: {},
                closeDeletePopupHandler: jest.fn()
            };
            let store = mockStore(initialState);

            let wrapper = shallow(
                <DeletePopup store={store} {...props} />
            );
            wrapper.instance().confirmHandler = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            wrapper.find('.btn-submit').simulate('click');

            // Assert
            expect(wrapper.instance().confirmHandler).toHaveBeenCalled();
        });

        it("when click Cancel - call cancelHandler", () => {
            // Arrange
            const initialState = { deletePopupData: { content: "some content" } };
            let props = {
                deleteData: {
                },
                deletePopupData: {},
                closeDeletePopupHandler: jest.fn()
            };
            let store = mockStore(initialState);

            let wrapper = shallow(
                <DeletePopup store={store} {...props} />
            );
            wrapper.instance().cancelHandler = jest.fn();
            wrapper.instance().forceUpdate();

            // Act
            wrapper.find('.btn-cancel').simulate('click');

            // Assert
            expect(wrapper.instance().cancelHandler).toHaveBeenCalled();
        });
    });     
});